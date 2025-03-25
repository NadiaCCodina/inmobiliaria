using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliaria.Models

{

    public class Propietario
    {
        [Key]
        [Display(Name = "Codigo")]
        public int PropietarioId{get; set;}
        [Required]
        public string Nombre { get; set; }
		[Required]
		public string Apellido { get; set; }
		[Required]
		public string Dni { get; set; }
		public string Telefono { get; set; }
		[Required, EmailAddress]

        public string Direccion {get; set;}
        [Required]
		public string Email { get; set; }

    }
}