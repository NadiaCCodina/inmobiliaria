using System.ComponentModel.DataAnnotations;

namespace inmobiliaria.Models
{
	public class Inquilino
	{
		[Key]
		[Display(Name = "Código")]
		public int Id { get; set; }
		[Required]
		public string Nombre { get; set; }
		[Required]
		public string Apellido { get; set; }
		[Required]
		public string Dni { get; set; }
		public string Telefono { get; set; }
		[Required, EmailAddress]
		public string Email { get; set; }
	
	   public override string ToString()
        {
            //return $"{Apellido}, {Nombre}";
            //return $"{Nombre} {Apellido}";
            var res = $"{Nombre} {Apellido}";
            if (!String.IsNullOrEmpty(Dni))
            {
                res += $" ({Dni})";
            }
            return res;
        }
    }
}

