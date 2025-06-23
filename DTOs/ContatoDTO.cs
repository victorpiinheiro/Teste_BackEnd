using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClienteApi.DTOs
{
    public class ContatoDTO
    {

        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Texto { get; set; }
        public int ClienteId { get; set; }
    }
    public class ContatoCreateDTO
    {
        public string Tipo { get; set; }
        public string Texto { get; set; }
        public int ClienteId { get; set; }
    }
    public class ContatoUpateDTO
    {
        [Required]
        public string Tipo { get; set; }
        [Required]
        public string Texto { get; set; }
        
    }
}