
using System.ComponentModel.DataAnnotations;

namespace ClienteApi.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataCadastro { get; set; }
    }
    public class ClienteCreateDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }


    }

    public class ClienteDetailsDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataCadastro { get; set; }
        public EnderecoDTO Endereco { get; set; }
        public List<ContatoDTO> Contatos { get; set; }
    }

    public class ClienteUpdateDTO
    {
        public string Nome { get; set; }
       
    }
}