﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PupuseriaSalvadorena.Conexion;
using PupuseriaSalvadorena.Models;
using PupuseriaSalvadorena.Repositorios.Implementaciones;
using PupuseriaSalvadorena.Repositorios.Interfaces;
using PupuseriaSalvadorena.ViewModels;
using Rotativa.AspNetCore;

namespace PupuseriaSalvadorena.Controllers
{
    public class DeclaracionImpuestoesController : Controller
    {
        private readonly IDeclaracionTaxRep _declaracionTaxRep;
        private readonly IDetallesTransacRep _detallesTransacRep;
        private readonly INegociosRep _negociosRep;

        public DeclaracionImpuestoesController(IDeclaracionTaxRep declaracionTaxRep, IDetallesTransacRep detallesTransacRep, INegociosRep negociosRep)
        {
            _declaracionTaxRep = declaracionTaxRep;
            _detallesTransacRep = detallesTransacRep;
            _negociosRep = negociosRep;
        }

        // GET: DeclaracionImpuestoes
        public async Task<IActionResult> Index()
        {
            var declaracionImpuesto = await _declaracionTaxRep.MostrarDeclaracionesImpuestos();
            return View(declaracionImpuesto);   
        }

        // GET: DeclaracionImpuestoes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Error al crear la declaracion." });
            }

            var negocio = await _negociosRep.MostrarNegocio();
            var declaracionImpuesto = await _declaracionTaxRep.ConsultarDeclaracionesImpuestos(id);
            var trimestre = declaracionImpuesto.Trimestre.Split(' ');
            var nTrimestre = int.Parse(trimestre[1]);
            var year = int.Parse(trimestre[2]);

            int mesInicio = (nTrimestre - 1) * 3 + 1;
            int mesFinal = nTrimestre * 3;

            var transacciones = (await _detallesTransacRep.MostrarDetallesTransaccionesYear())
                                .Where(t => t.FechaTrans.Year == year && t.FechaTrans.Month >= mesInicio && t.FechaTrans.Month <= mesFinal).ToList();

            if (declaracionImpuesto == null)
            {
                return Json(new { success = false, message = "Error al crear la declaracion." });
            }

            var viewModel = new Declaraciones
            {
                Negocio = negocio,
                DeclaracionImpuesto = declaracionImpuesto,
                DetallesTransacciones = transacciones
            };

            return View(viewModel);
        }

        // GET: DeclaracionImpuestoes/Create
        public IActionResult Create()
        {
            return PartialView("_newDeclaracionPartial", new Presupuesto());
        }

        // POST: DeclaracionImpuestoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDeclaracionImpuesto,CedulaJuridica,FechaInicio,Trimestre,MontoRenta,MontoIVA,MontoTotalImpuestos,MontoTotal,Observaciones")] DeclaracionImpuesto declaracionImpuesto)
        {
            if (ModelState.IsValid)
            {
                var trimestre = declaracionImpuesto.Trimestre.Split(' ');
                var nTrimestre = int.Parse(trimestre[1]);
                var year = int.Parse(trimestre[2]);

                int mesInicio = (nTrimestre - 1) * 3 + 1;
                int mesFinal = nTrimestre * 3;

                var transacciones = (await _detallesTransacRep.MostrarDetallesTransaccionesYear())
                                    .Where(t => t.FechaTrans.Year == year && t.FechaTrans.Month >= mesInicio && t.FechaTrans.Month <= mesFinal).ToList();

                decimal montoTotal = transacciones.Sum(t => t.Monto);
                decimal montoRenta = montoTotal * 0.02m;
                decimal montoIVA = montoTotal * 0.04m;
                decimal montoTotalImpuestos = montoRenta + montoIVA;

                var negocio = await _negociosRep.ConsultarNegocio();
                var IdDeclaracion = await _declaracionTaxRep.CrearDeclaracionTax(negocio, declaracionImpuesto.FechaInicio, declaracionImpuesto.Trimestre, montoRenta, montoIVA, montoTotalImpuestos, montoTotal, declaracionImpuesto.Observaciones);

                return Json(new { success = true, url = Url.Action(nameof(Details), new { id = IdDeclaracion }) });
            }
            return Json(new { success = false, message = "Error al crear la declaracion." });
        }

        // GET: DeclaracionImpuestoes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Error al editar la declaracion." });
            }

            var declaracionImpuesto = await _declaracionTaxRep.ConsultarDeclaracionesImpuestos(id);

            if (declaracionImpuesto == null)
            {
                return Json(new { success = false, message = "Error al editar la declaracion." });
            }
            return PartialView("_editDeclaracionPartial", declaracionImpuesto);
        }

        // POST: DeclaracionImpuestoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IdDeclaracionImpuesto,CedulaJuridica,FechaInicio,Trimestre,MontoRenta,MontoIVA,MontoTotalImpuestos,MontoTotal,Observaciones")] DeclaracionImpuesto declaracionImpuesto)
        {
            if (ModelState.IsValid)
            {
                await _declaracionTaxRep.ActualizarDeclaracionTax(id, declaracionImpuesto.Observaciones);
                return Json(new { success = true, message = "Declaracion editada correctamente." });
            }
            return Json(new { success = false, message = "Error al editar la declaracion." });
        }

        // GET: DeclaracionImpuestoes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var declaracionImpuesto = await _declaracionTaxRep.ConsultarDeclaracionesImpuestos(id);
            return Json(new { exists = declaracionImpuesto != null });
        }

        // POST: DeclaracionImpuestoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _declaracionTaxRep.EliminarDeclaracionTax(id);
                return Json(new { success = true, url = Url.Action(nameof(Index)) });
            }
            catch
            {
                return Json(new { success = false, message = "Error al eliminar la declaracion." });
            }
        }

        // DeclaracionImpuestoes/ExcelDeclaracion/5
        public async Task<IActionResult> ExcelDeclaracion(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var declaracionImpuesto = await _declaracionTaxRep.ConsultarDeclaracionesImpuestos(id);
            var negocio = await _negociosRep.MostrarNegocio();
            if (declaracionImpuesto == null)
            {
                return NotFound();
            }

            var trimestre = declaracionImpuesto.Trimestre.Split(' ');
            var nTrimestre = int.Parse(trimestre[1]);
            var year = int.Parse(trimestre[2]);

            int mesInicio = (nTrimestre - 1) * 3 + 1;
            int mesFinal = nTrimestre * 3;

            var transacciones = (await _detallesTransacRep.MostrarDetallesTransaccionesYear())
                                .Where(t => t.FechaTrans.Year == year && t.FechaTrans.Month >= mesInicio && t.FechaTrans.Month <= mesFinal).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Declaracion");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Valor Total";
                worksheet.Cell(currentRow, 2).Value = "Impuestos de Renta (2%)";
                worksheet.Cell(currentRow, 3).Value = "Impuestos IVA (4%)";
                worksheet.Cell(currentRow, 4).Value = "Impuestos a Pagar";

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = declaracionImpuesto.MontoTotal;
                worksheet.Cell(currentRow, 2).Value = declaracionImpuesto.MontoRenta;
                worksheet.Cell(currentRow, 3).Value = declaracionImpuesto.MontoIVA;
                worksheet.Cell(currentRow, 4).Value = declaracionImpuesto.MontoTotalImpuestos;

                currentRow += 2;
                worksheet.Cell(currentRow, 1).Value = "ID Factura";
                worksheet.Cell(currentRow, 2).Value = "Fecha";
                worksheet.Cell(currentRow, 3).Value = "Descripcion";
                worksheet.Cell(currentRow, 4).Value = "Cantidad";
                worksheet.Cell(currentRow, 5).Value = "Debito";
                worksheet.Cell(currentRow, 6).Value = "Tipo de Transaccion";

                foreach (var transaccionesImpuestos in transacciones)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = transaccionesImpuestos.IdTransaccion;
                    worksheet.Cell(currentRow, 2).Value = transaccionesImpuestos.FechaTrans.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 3).Value = transaccionesImpuestos.DescripcionTransaccion;
                    worksheet.Cell(currentRow, 4).Value = transaccionesImpuestos.Cantidad;
                    worksheet.Cell(currentRow, 5).Value = transaccionesImpuestos.Monto;
                    worksheet.Cell(currentRow, 6).Value = transaccionesImpuestos.TipoTransac;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Declaracion {declaracionImpuesto.Trimestre}.xlsx");
                }
            }
        }

        public async Task<IActionResult> DescargarDeclaracion(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var declaracionImpuesto = await _declaracionTaxRep.ConsultarDeclaracionesImpuestos(id);
            var negocio = await _negociosRep.MostrarNegocio();
            if (declaracionImpuesto == null)
            {
                return NotFound();
            }

            var trimestre = declaracionImpuesto.Trimestre.Split(' ');
            var nTrimestre = int.Parse(trimestre[1]);
            var year = int.Parse(trimestre[2]);

            int mesInicio = (nTrimestre - 1) * 3 + 1;
            int mesFinal = nTrimestre * 3;

            var transacciones = (await _detallesTransacRep.MostrarDetallesTransaccionesYear())
                                .Where(t => t.FechaTrans.Year == year && t.FechaTrans.Month >= mesInicio && t.FechaTrans.Month <= mesFinal).ToList();

            var viewModel = new Declaraciones
            {
                Negocio = negocio,
                DeclaracionImpuesto = declaracionImpuesto,
                DetallesTransacciones = transacciones
            };

            return new ViewAsPdf("DescargarDeclaracion", viewModel)
            {
                FileName = $"Declaracion_ D105_{declaracionImpuesto.Trimestre}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.Letter,
                PageMargins = new Rotativa.AspNetCore.Options.Margins(15, 12, 12, 12)
            };
        }
    }
}
