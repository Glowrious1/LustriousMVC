namespace Lustrious.Models
{
    public class Usuario
    {
        public int IdUser { get; set; }
        public string Nome { get; set; }
        public string Foto  { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Sexo { get; set; }
        public string CPF{ get; set; }
        public string Role { get; set; }
        public int CEP { get; set; }

        public string Ativo { get; set; }

    }
}
