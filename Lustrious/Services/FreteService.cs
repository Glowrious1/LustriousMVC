using Lustrious.Data;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System;
using System.Linq;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;

namespace Lustrious.Services
{
 public class FreteService : IFreteService
 {
 private readonly DataBase _dataBase;
 private readonly IHttpClientFactory _httpClientFactory;
 private readonly string _mapsApiKey;
 private readonly decimal _baseRate;
 private readonly decimal _perKm;
 private readonly string _distributorAddress;
 private readonly ILogger<FreteService> _logger;
 private readonly string _provider;
 private readonly string _orsApiKey;

 public FreteService(DataBase dataBase, IHttpClientFactory httpClientFactory, Microsoft.Extensions.Configuration.IConfiguration configuration, ILogger<FreteService> logger)
 {
 _dataBase = dataBase;
 _httpClientFactory = httpClientFactory;
 _mapsApiKey = configuration["Maps:ApiKey"] ?? Environment.GetEnvironmentVariable("MAPS_API_KEY");
 _baseRate = decimal.TryParse(configuration["Frete:BaseRate"], out var br) ? br :10m;
 _perKm = decimal.TryParse(configuration["Frete:PerKm"], out var pk) ? pk :2.5m;
 _distributorAddress = configuration["Frete:DistributorAddress"] ?? "R. Guaipá,678 - Vila Leopoldina, São Paulo - SP,05089-000";
 _logger = logger;
 _provider = configuration["Frete:Provider"] ?? "OpenRouteService";
 _orsApiKey = configuration["OpenRouteService:ApiKey"] ?? Environment.GetEnvironmentVariable("ORS_API_KEY");
 }

 public async Task<decimal> CalcularFreteAsync(int enderecoClienteId)
 {
 _logger.LogInformation("CalcularFreteAsync called for enderecoId={EnderecoId} provider={Provider}", enderecoClienteId, _provider);

 // try provider
 if (_provider == "OpenRouteService" && !string.IsNullOrWhiteSpace(_orsApiKey))
 {
 try
 {
 //1) geocode distributor address and customer cep -> coordinates
 var customerCep = GetCepByEnderecoId(enderecoClienteId);
 if (string.IsNullOrWhiteSpace(customerCep))
 {
 _logger.LogWarning("CEP do cliente não encontrado para enderecoId={EnderecoId}", enderecoClienteId);
 return _baseRate;
 }
 var distributorCoords = await GeocodeAddressOrCoordsAsync(_distributorAddress, isAddress: true);
 var customerCoords = await GeocodeAddressOrCoordsAsync(customerCep, isAddress: true);
 if (distributorCoords == null || customerCoords == null)
 {
 _logger.LogWarning("Não foi possível obter coordenadas via ORS");
 }
 else
 {
 var distanceMeters = await GetDistanceMetersORS(distributorCoords.Value, customerCoords.Value);
 if (distanceMeters >=0)
 {
 decimal km = Math.Max(0.1m, distanceMeters /1000m);
 var valor = Math.Round(_baseRate + _perKm * km,2);
 _logger.LogInformation("Frete calculado via ORS: {Valor} para {Km} km", valor, km);
 return valor;
 }
 }
 }
 catch(Exception ex)
 {
 _logger.LogError(ex, "Erro ao calcular frete via ORS");
 }
 }

 // fallback: if google configured try distance matrix
 if (!string.IsNullOrWhiteSpace(_mapsApiKey))
 {
 _logger.LogInformation("Maps api key presente, tentando Google Distance Matrix como fallback");
 try
 {
 var customerCep = GetCepByEnderecoId(enderecoClienteId);
 if (!string.IsNullOrWhiteSpace(customerCep))
 {
 var origin = Uri.EscapeDataString(_distributorAddress);
 var destination = Uri.EscapeDataString(customerCep);
 var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&key={_mapsApiKey}";
 var client = _httpClientFactory.CreateClient();
 var resp = await client.GetStringAsync(url);
 using var doc = JsonDocument.Parse(resp);
 var root = doc.RootElement;
 if (root.TryGetProperty("rows", out var rows) && rows.GetArrayLength() >0)
 {
 var elements = rows[0].GetProperty("elements");
 if (elements.GetArrayLength() >0)
 {
 var element = elements[0];
 var status = element.TryGetProperty("status", out var statusProp) ? statusProp.GetString() : null;
 if (status == "OK")
 {
 var distanceMeters = element.GetProperty("distance").GetProperty("value").GetInt32();
 decimal km = Math.Max(0.1m, distanceMeters /1000m);
 var valor = Math.Round(_baseRate + _perKm * km,2);
 _logger.LogInformation("Frete calculado via Google DM fallback: {Valor}", valor);
 return valor;
 }
 }
 }
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Erro no fallback do Google Distance Matrix");
 }
 }

 // heurístico final
 var heuristic = CalculateFreteHeuristic(enderecoClienteId);
 _logger.LogInformation("Frete calculado por heurístico final: valor={Valor}", heuristic);
 return heuristic;
 }

 private async Task<(double lat, double lon)?> GeocodeAddressOrCoordsAsync(string input, bool isAddress)
 {
 // if input looks like coordinates, parse
 var digits = new string(input.Where(c => char.IsDigit(c) || c == '.' || c == ',' || c == '-').ToArray());
 if (!isAddress && digits.Contains(','))
 {
 var parts = digits.Split(',');
 if (parts.Length >=2 && double.TryParse(parts[0], out var la) && double.TryParse(parts[1], out var lo))
 return (la, lo);
 }

 // use ORS geocoding
 try
 {
 var client = _httpClientFactory.CreateClient();
 var url = $"https://api.openrouteservice.org/geocode/search?api_key={_orsApiKey}&text={Uri.EscapeDataString(input)}";
 var resp = await client.GetStringAsync(url);
 using var doc = JsonDocument.Parse(resp);
 var root = doc.RootElement;
 if (root.TryGetProperty("features", out var features) && features.GetArrayLength() >0)
 {
 var first = features[0];
 var coords = first.GetProperty("geometry").GetProperty("coordinates");
 double lon = coords[0].GetDouble();
 double lat = coords[1].GetDouble();
 return (lat, lon);
 }
 }
 catch(Exception ex)
 {
 _logger.LogError(ex, "Erro ao geocodificar via ORS");
 }
 return null;
 }

 private async Task<int> GetDistanceMetersORS((double lat, double lon) origin, (double lat, double lon) destination)
 {
 try
 {
 var client = _httpClientFactory.CreateClient();
 var url = "https://api.openrouteservice.org/v2/matrix/driving-car";
 var payload = new
 {
 locations = new double[][] { new double[] { origin.lon, origin.lat }, new double[] { destination.lon, destination.lat } },
 metrics = new string[] { "distance" }
 };
 var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
 var req = new HttpRequestMessage(HttpMethod.Post, url);
 req.Headers.Add("Authorization", _orsApiKey);
 req.Content = content;
 var resp = await client.SendAsync(req);
 resp.EnsureSuccessStatusCode();
 var txt = await resp.Content.ReadAsStringAsync();
 using var doc = JsonDocument.Parse(txt);
 var root = doc.RootElement;
 if (root.TryGetProperty("distances", out var distances))
 {
 // distances is matrix: distances[0][1]
 var d = distances[0][1].EnumerateArray();
 // manually parse
 var arr0 = distances[0];
 if (arr0.GetArrayLength() >1)
 {
 var meters = arr0[1].GetDouble();
 return Convert.ToInt32(meters);
 }
 }
 }
 catch(Exception ex)
 {
 _logger.LogError(ex, "Erro ao chamar ORS matrix");
 }
 return -1;
 }

 private string GetCepByEnderecoId(int enderecoId)
 {
 try
 {
 using var conn = _dataBase.GetConnection();
 using var cmd = new MySqlCommand("SELECT cep FROM endereco WHERE id_endereco = @id", conn);
 cmd.Parameters.AddWithValue("@id", enderecoId);
 var result = cmd.ExecuteScalar();
 var cep = result == null || result == DBNull.Value ? string.Empty : result.ToString();
 _logger.LogDebug("GetCepByEnderecoId id={Id} cep={Cep}", enderecoId, cep);
 return cep;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Erro ao obter CEP para enderecoId={EnderecoId}", enderecoId);
 return string.Empty;
 }
 }

 private decimal CalculateFreteHeuristic(int customerAddressId)
 {
 try
 {
 string customerCep = GetCepByEnderecoId(customerAddressId);
 string distributorCep = new string(_distributorAddress.Where(char.IsDigit).ToArray());
 if (string.IsNullOrWhiteSpace(customerCep) || string.IsNullOrWhiteSpace(distributorCep))
 return _baseRate;
 var c1 = long.Parse(new string(customerCep.Where(char.IsDigit).ToArray()));
 var c2 = long.Parse(new string(distributorCep.Where(char.IsDigit).ToArray()));
 var diff = Math.Abs(c1 - c2);
 var valor = Math.Max(_baseRate, (decimal)(diff %100));
 return Math.Round(valor,2);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Erro no heurístico de frete para enderecoId={EnderecoId}", customerAddressId);
 return _baseRate;
 }
 }
 }
}
