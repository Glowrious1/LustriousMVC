using Lustrious.Data;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System;
using System.Linq;
using MySql.Data.MySqlClient;

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

 public FreteService(DataBase dataBase, IHttpClientFactory httpClientFactory, Microsoft.Extensions.Configuration.IConfiguration configuration)
 {
 _dataBase = dataBase;
 _httpClientFactory = httpClientFactory;
 _mapsApiKey = configuration["Maps:ApiKey"] ?? Environment.GetEnvironmentVariable("MAPS_API_KEY");
 _baseRate = decimal.TryParse(configuration["Frete:BaseRate"], out var br) ? br :10m;
 _perKm = decimal.TryParse(configuration["Frete:PerKm"], out var pk) ? pk :2.5m;
 _distributorAddress = configuration["Frete:DistributorAddress"] ?? "R. Guaipá,678 - Vila Leopoldina, São Paulo - SP,05089-000";
 }

 public async Task<decimal> CalcularFreteAsync(int enderecoClienteId)
 {
 // tentar Google Distance Matrix
 if (!string.IsNullOrWhiteSpace(_mapsApiKey))
 {
 try
 {
 var customerCep = GetCepByEnderecoId(enderecoClienteId);
 if (string.IsNullOrWhiteSpace(customerCep))
 return _baseRate;
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
 if (element.TryGetProperty("status", out var status) && status.GetString() == "OK")
 {
 var distanceMeters = element.GetProperty("distance").GetProperty("value").GetInt32();
 decimal km = Math.Max(0.1m, distanceMeters /1000m);
 var valor = Math.Round(_baseRate + _perKm * km,2);
 return valor;
 }
 }
 }
 }
 catch
 {
 // fallback
 }
 }

 return CalculateFreteHeuristic(enderecoClienteId);
 }

 private string GetCepByEnderecoId(int enderecoId)
 {
 using var conn = _dataBase.GetConnection();
 using var cmd = new MySqlCommand("SELECT cep FROM endereco WHERE id_endereco = @id", conn);
 cmd.Parameters.AddWithValue("@id", enderecoId);
 var result = cmd.ExecuteScalar();
 return result == null || result == DBNull.Value ? string.Empty : result.ToString();
 }

 private decimal CalculateFreteHeuristic(int customerAddressId)
 {
 string customerCep = GetCepByEnderecoId(customerAddressId);
 string distributorCep = new string(_distributorAddress.Where(char.IsDigit).ToArray());
 if (string.IsNullOrWhiteSpace(customerCep) || string.IsNullOrWhiteSpace(distributorCep))
 return _baseRate;
 try
 {
 var c1 = long.Parse(new string(customerCep.Where(char.IsDigit).ToArray()));
 var c2 = long.Parse(new string(distributorCep.Where(char.IsDigit).ToArray()));
 var diff = Math.Abs(c1 - c2);
 var valor = Math.Max(_baseRate, (decimal)(diff %100));
 return Math.Round(valor,2);
 }
 catch
 {
 return _baseRate;
 }
 }
 }
}
