using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliaria.Models
{
	[Table("Contrato")]
	public class Contrato
	{

		public int Id { get; set; }
		[Required]
		public int InquilinoId { get; set; }
		[ForeignKey(nameof(InquilinoId))]
		[Required]
		public int InmuebleId { get; set; }
		[ForeignKey(nameof(InmuebleId))]
		public decimal Monto { get; set; }
		//[Required]
		public DateTime FechaInicio { get; set; }
		//[Required]
		public DateTime FechaFin { get; set; }
		public Inquilino? Inqui { get; set; }
		public Inmueble? Inmue { get; set; }

		public int Pagos { get; set; }
		
		
      
 
	}
}