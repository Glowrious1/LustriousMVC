namespace Lustrious.Models
{
 public class Notificacao
 {
 public int Id { get; set; }
 public int IdUser { get; set; }
 public string Mensagem { get; set; }
 public DateTime DataEnvio { get; set; }
 public bool Lida { get; set; }
 }
}
