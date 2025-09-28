namespace Lustrious.Models
{
    public class Cliente
    {
        public int IdClient { get; set; }
        public string Nome { get; set; }

        public string Email { get; set; }

        public int CPF { get; set; }

        public string Senha { get; set; }

        public int CepCli {  get; set; }
    }
}
