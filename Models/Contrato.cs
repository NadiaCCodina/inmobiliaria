using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

		public DateTime? FechaFinEfectiva { get; set; }
		public Inquilino? Inqui { get; set; }
		public Inmueble? Inmue { get; set; }

		public int Pagos { get; set; }
		public int? UsuarioAltaId { get; set; }
		public DateTime? FechaAltaUsuario { get; set; }
		public int? UsuarioBajaId { get; set; }
		public DateTime? FechaFinUsuario { get; set; }
		public Usuario? UsuAlta { get; set; }
		public Usuario? UsuBaja { get; set; }
		
		
      
 
	}
}