using Lustrious.Data;
using System.Threading.Tasks;
using System;
using System.Linq;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Lustrious.Services
{
public class FreteService : IFreteService
{
private readonly DataBase _dataBase;
private readonly decimal _baseRate;
private readonly decimal _perKm;
private readonly string _distributorAddress;
private readonly ILogger<FreteService> _logger;

public FreteService(DataBase dataBase, IConfiguration configuration, ILogger<FreteService> logger)
{
_dataBase = dataBase;
_baseRate = decimal.TryParse(configuration["Frete:BaseRate"], out var br) ? br :10m;
_perKm = decimal.TryParse(configuration["Frete:PerKm"], out var pk) ? pk :2.5m;
_distributorAddress = configuration["Frete:DistributorAddress"] ?? "R. Guaipê,678 - Vila Leopoldina, São Paulo - SP,05089-000";
_logger = logger;
}

public Task<decimal> CalcularFreteAsync(int enderecoClienteId)
{
_logger.LogInformation("CalcularFreteAsync - heurístico para enderecoId={EnderecoId}", enderecoClienteId);
var valor = CalculateFreteHeuristic(enderecoClienteId);
_logger.LogInformation("Frete heurístico = {Valor}", valor);
return Task.FromResult(valor);
}

private string GetCepByEnderecoId(int enderecoId)
{
try
{
using var conn = _dataBase.GetConnection();
conn.Open();
using var cmd = new MySqlCommand("SELECT cep FROM endereco WHERE id_endereco = @id", conn);
cmd.Parameters.AddWithValue("@id", enderecoId);
var result = cmd.ExecuteScalar();
var cep = result == null || result == DBNull.Value ? string.Empty : result.ToString();
_logger.LogDebug("GetCepByEnderecoId id={Id} cep={Cep}", enderecoId, cep);
return cep ?? string.Empty;
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

var digitsCustomer = new string(customerCep.Where(char.IsDigit).ToArray());
if (!long.TryParse(digitsCustomer, out var c1))
return _baseRate;
if (!long.TryParse(distributorCep, out var c2))
return _baseRate;

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
