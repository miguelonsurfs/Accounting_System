﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PupuseriaSalvadorena.Conexion;
using PupuseriaSalvadorena.Models;
using PupuseriaSalvadorena.Repositorios.Interfaces;
using System.Data;

namespace PupuseriaSalvadorena.Repositorios.Implementaciones
{
    public class FacturaCompraRep : IFacturaCompraRep
    {
        private readonly MiDbContext _context;

        public FacturaCompraRep(MiDbContext context)
        {
            _context = context;
        }

        public async Task EliminarFacturaCompra(string IdFacturaCompra)
        {
            var IdFacturaCompraParam = new SqlParameter("@IdFacturaCompra", IdFacturaCompra);
            await _context.Database.ExecuteSqlRawAsync("EliminarFacturaCompra @IdFacturaCompra", IdFacturaCompraParam);
        }

        public async Task<List<FacturaCompra>> MostrarFacturasCompras()
        {
            var FacturasCompras = await _context.FacturaCompra
                                        .FromSqlRaw("EXEC MostrarFacturasCompras")
                                        .ToListAsync();
            return FacturasCompras;
        }

        public async Task<FacturaCompra> ConsultarFacturasCompras(string IdFacturaCompra)
        {
            var IdFacturaCompraParam = new SqlParameter("@IdFacturaCompra", IdFacturaCompra);
            var resultado = await _context.FacturaCompra
                                          .FromSqlRaw("EXEC ConsultarFacturasCompras @IdFacturaCompra", IdFacturaCompraParam)
                                          .ToListAsync();

            return resultado.FirstOrDefault();
        }

        public async Task<string> CrearFacturaId(byte[] FacturaCom, DateTime FechaFactura, decimal TotalCompra, string DetallesCompra, int IdTipoPago, int IdTipoFactura, int IdMateriaPrima)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "CrearFacturaId";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@FacturaCom", FacturaCom ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@FechaFactura", FechaFactura));
                command.Parameters.Add(new SqlParameter("@TotalCompra", TotalCompra));
                command.Parameters.Add(new SqlParameter("@DetallesCompra", DetallesCompra ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@IdTipoPago", IdTipoPago));
                command.Parameters.Add(new SqlParameter("@IdTipoFactura", IdTipoFactura));
                command.Parameters.Add(new SqlParameter("@IdMateriaPrima", IdMateriaPrima));

                var IdFactura = new SqlParameter("@IdFactura", SqlDbType.VarChar, 10)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(IdFactura);

                await _context.Database.OpenConnectionAsync();
                await command.ExecuteNonQueryAsync();

                return (string)IdFactura.Value;
            }
        }
    }
}
