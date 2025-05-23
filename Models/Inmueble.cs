﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria.Models
{
	[Table("Inmueble")]
	public class Inmueble
	{
		
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
		public Contrato? ContratoInmueble { get; set; }

		 public override string ToString()
        {
          
            var res = $"Direccion: {Direccion} Precio: {Precio}";
           
            return res;
        }
	}
}
