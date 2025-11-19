using System.ComponentModel.DataAnnotations;

namespace Lustrious.Models
{
    public class Endereco
    {
        public int IdEndereco { get; set; }

        [Required(ErrorMessage = "CEP é obrigatório")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP inválido")]
        public string Cep { get; set; }

        [Required(ErrorMessage = "Logradouro é obrigatório")]
        [MaxLength(200)]
        public string logradouro { get; set; }

        [Required(ErrorMessage = "Número é obrigatório")]
        [MaxLength(20)]
        public string numero { get; set; }

        [MaxLength(100)]
        public string complemento { get; set; }
        public int Idbairro { get; set; }
        public int Idcidade { get; set; }
        public int Idestado { get; set; }

        public int IdUser { get; set; }

    }
}
