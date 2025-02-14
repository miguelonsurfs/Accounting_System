﻿using System.ComponentModel.DataAnnotations;

namespace PupuseriaSalvadorena.Models
{
    public class DetalleCuenta
    {
        [Key]
        public string? IdDetallesCuenta { get; set; }

        [Required(ErrorMessage = "El monto pagado es obligatorio")]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})?$", ErrorMessage = "El monto pagado no es válido")]
        public decimal Pago { get; set; }

        public DateTime FechaIngreso { get; set; }

        [Required(ErrorMessage = "El id de la cuenta por pagar es obligatorio")]
        public string? IdCuentaPagar { get; set; }
    }
}
