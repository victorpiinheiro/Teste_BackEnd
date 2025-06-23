using System.ComponentModel.DataAnnotations;

namespace ClienteApi.DTOs
{
    public class EnderecoDTO
    {

       public int ClienteId { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Cidade { get; set; }
        public string Numero { get; set; }
        public string? Complemento { get; set; }
        
    }

    public class EnderecoCreateDTO
    {
        [Required]
        public string Cep { get; set; }
        public string Numero { get; set; }
        public string? Complemento { get; set; }
        [Required]
        public int ClienteId { get; set; }


    }


    public class EnderecoUpdateCompletoDTO
    {
        [Required(ErrorMessage = "Cep é obrigatório")]
        public string Cep { get; set; }

        [Required(ErrorMessage = "Número é obrigatório")]
        public string Numero { get; set; }
        public string? Complemento { get; set; }

    }
    public class EnderecoUpdateParcialDTO
    {
        [Required(ErrorMessage = "O numero é obrigatório")]
        public string Numero { get; set; }
        public string? Complemento { get; set; }

    }
}