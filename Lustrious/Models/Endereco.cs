namespace Lustrious.Models
{
    public class Endereco
    {
        public int IdEndereco { get; set; }
        public string Cep { get; set; }

        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public int Idbairro { get; set; }
        public int Idcidade { get; set; }
        public int Idestado { get; set; }
        
        public int IdUser   { get; set; }

    }
}
