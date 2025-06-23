
namespace ClienteApi.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        
        public Endereco Endereco { get; set; }

        public List<Contato> Contatos { get; set; }
    }
}