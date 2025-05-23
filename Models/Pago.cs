using System.ComponentModel.DataAnnotations.Schema;


namespace inmobiliaria.Models
{
    [Table("Pago")]
    public class Pago
    {

        public int Id { get; set; }

        public int ContratoId { get; set; }
        [ForeignKey(nameof(ContratoId))]

        public int NroCuota { get; set; }

        public decimal Monto { get; set; }

        public DateTime? FechaPago { get; set; }

        public String? Mes { get; set; }

        public String? Estado { get; set; }

		public int? UsuarioAltaId { get; set; }

		public DateTime? FechaAltaUsuario { get; set; }

		public int? UsuarioBajaId { get; set; }

		public DateTime? FechaFinUsuario { get; set; }

		public Usuario? UsuAlta { get; set; }

		public Usuario? UsuBaja { get; set; }
		
    }
}