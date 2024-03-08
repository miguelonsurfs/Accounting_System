﻿using PupuseriaSalvadorena.Models;

namespace PupuseriaSalvadorena.Repositorios.Interfaces
{
    public interface ICantonesRep
    {
        Task CrearCanton(string nombre, int provincia);
        Task ActualizarCanton(int id, string nombre, int provincia);
        Task EliminarCanton(int id);
        Task<List<Canton>> MostrarCantones();
        Task<List<Canton>> ConsultarCantones(int id);
        Task<Canton> ConsultarCanton(int id);
    }
}
