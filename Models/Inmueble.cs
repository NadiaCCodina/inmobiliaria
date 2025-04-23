using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliaria.Models
{
	[Table("Inmueble")]
	public class Inmueble
	{
		[Display(Name = "Nº")]
		public int Id { get; set; }
		//[Required]
		[Display(Name = "Dirección")]
		public string? Direccion { get; set; }
		[Required]
		public int Ambientes { get; set; }
		[Required]
		public int Superficie { get; set; }
		public decimal Latitud { get; set; }
		public decimal Longitud { get; set; }
		[Display(Name = "Dueño")]
		public int PropietarioId { get; set; }
		[ForeignKey(nameof(PropietarioId))]
		public string? Uso { get; set; }
		[Required]
		public string? Tipo { get; set; }
		[Required]
		public int Precio { get; set; }
	
		public Propietario? Duenio { get; set; }

		 public override string ToString()
        {
            //return $"{Apellido}, {Nombre}";
            //return $"{Nombre} {Apellido}";
            var res = $"Direccion: {Direccion} Precio: {Precio}";
           
            return res;
        }
	}
}
