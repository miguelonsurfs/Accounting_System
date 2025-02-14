﻿using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PupuseriaSalvadorena.Models
{
    public class DetalleTransaccion
    {
        public string? IdRegistroLibros { get; set; }

        public int IdTransaccion { get; set; }

        [Required(ErrorMessage = "Se requiere una descripción de la transacción.")]
        public string? DescripcionTransaccion { get; set; }

        [Required(ErrorMessage = "La cantidad del producto es obligatorio")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "La cantidad del producto solo puede contener números")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio del producto es obligatorio")]
        [RegularExpression(@"^[0-9]+(\.[0-9]+)?$", ErrorMessage = "El monto solo puede contener números")]
        public decimal Monto { get; set; }

        public DateTime FechaTrans { get; set; }

        [Required(ErrorMessage = "El tipo de transaccion es obligatorio")]
        public int? IdTipo { get; set; }

        [Required(ErrorMessage = "El impuesto asociado es obligatorio")]
        public string? IdImpuesto { get; set; }

        public string? NombreImpuesto { get; set; }

        public string? TipoTransac { get; set; }

        public int IdMovimiento { get; set; }

        public string? NombreMov { get; set; }

        public bool Recurrencia { get; set; }

        public DateTime FechaRecurrencia { get; set; }

        public string Frecuencia { get; set; }

        public bool Conciliado { get; set; }
    }
}
