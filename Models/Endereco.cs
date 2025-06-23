

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClienteApi.Models
{
  public class Endereco
  {

    [Key]

    public int ClienteId { get; set; }
    public string Cep { get; set; }
    public string Logradouro { get; set; }
    public string Cidade { get; set; }
    public string Numero { get; set; }
    public string Complemento { get; set; }
    [ForeignKey("ClienteId")]
    public Cliente Cliente { get; set; }

  }
}