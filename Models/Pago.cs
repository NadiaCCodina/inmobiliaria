using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace inmobiliaria.Models
{
    	[Table("Pago")]
    public class Pago{
       
		public int Id { get; set; }
		
		public int ContratoId { get; set; }
		[ForeignKey(nameof(ContratoId))]
      
		public int NroCuota { get; set; }

        public decimal Monto { get; set; }
		
        public DateTime FechaPago { get; set; }
		
        public String Mes { get; set; }
        
        public String Estado { get; set; }
		
    }
}