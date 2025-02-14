﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PupuseriaSalvadorena.Conexion;
using PupuseriaSalvadorena.Models;
using PupuseriaSalvadorena.Repositorios.Interfaces;
using System.Data;

namespace PupuseriaSalvadorena.Repositorios.Implementaciones
{
    public class RegistroLibrosRep : IRegistroLibrosRep
    {
        private readonly MiDbContext _context;

        public RegistroLibrosRep(MiDbContext context)
        {
            _context = context;
        }

        public async Task CrearRegistroLibros(DateTime FechaRegistro, decimal MontoTotal, string Descripcion, bool Conciliado)
        {
            var FechaRegistroParam = new SqlParameter("@FechaRegistro", FechaRegistro);
            var MontoTotalParam = new SqlParameter("@MontoTotal", MontoTotal);
            var DescripcionParam = new SqlParameter("@Descripcion", Descripcion);
            var ConciliadoParam = new SqlParameter("@Conciliado", Conciliado);
            await _context.Database.ExecuteSqlRawAsync("CrearRegistroLibros @FechaRegistro, @MontoTotal, @Descripcion, @Conciliado", FechaRegistroParam, MontoTotalParam, DescripcionParam, ConciliadoParam);
        }

        public async Task ActualizarRegistroLibros(string IdRegistroLibros, decimal MontoTotal, string Descripcion, bool Conciliado)
        {
            var IdRegistroLibrosParam = new SqlParameter("@IdRegistroLibros", IdRegistroLibros);
            var MontoTotalParam = new SqlParameter("@MontoTotal", MontoTotal);
            var DescripcionParam = new SqlParameter("@Descripcion", Descripcion);
            var ConciliadoParam = new SqlParameter("@Conciliado", Conciliado);
            await _context.Database.ExecuteSqlRawAsync("ActualizarRegistroLibros @IdRegistroLibros, @MontoTotal, @Descripcion, @Conciliado", IdRegistroLibrosParam, MontoTotalParam, DescripcionParam, ConciliadoParam);
        }

        public async Task EliminarRegistroLibros(string IdRegistroLibros)
        {
            var IdRegistroLibrosParam = new SqlParameter("@IdRegistroLibros", IdRegistroLibros);
            await _context.Database.ExecuteSqlRawAsync("EliminarRegistroLibros @IdRegistroLibros", IdRegistroLibrosParam);
        }

        public async Task<List<RegistroLibro>> MostrarRegistrosLibros()
        {
            var registrosLibros = await _context.RegistroLibro
                                        .FromSqlRaw("EXEC MostrarRegistrosLibros")
                                        .ToListAsync();
            return registrosLibros;
        }

        public async Task<RegistroLibro> ConsultarRegistrosLibros(string IdRegistroLibros)
        {
            var IdRegistroLibrosParam = new SqlParameter("@IdRegistroLibros", IdRegistroLibros);
            var resultado = await _context.RegistroLibro
                                          .FromSqlRaw("EXEC ConsultarRegistrosLibros @IdRegistroLibros", IdRegistroLibrosParam)
                                          .ToListAsync();

            return resultado.FirstOrDefault();
        }
    }
}
