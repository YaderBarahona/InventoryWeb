using Inventory.Models;
using InventoryWeb.Data.Repositorio.IRepositorio;
using InventoryWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class CategoriaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Categoria categoria = new Categoria();
            if (id == null)
            {
                //Esto es para crear una nueva categoria (insert)
                return View(categoria);
            }
            //actualizamos un registro Categoria existente
            categoria = _unidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if (categoria.CategoriaId == 0) //nuevo registro
                {
                    TempData["crear"] = "Categoría creada correctamente!!";
                    _unidadTrabajo.Categoria.Agregar(categoria);
                }
                else
                {
                    TempData["actualizar"] = "Categoría actualizada correctamente!!";
                    _unidadTrabajo.Categoria.Actualizar(categoria);
                }
                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }
        #region API
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _unidadTrabajo.Categoria.ObtenerTodos();
            return Json(new { data = todos });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoriaDb = _unidadTrabajo.Categoria.Obtener(id);
            if (categoriaDb == null)
            {
                return Json(new { success = false, message = "Error al borrar la categoría " });
            }
            else
                _unidadTrabajo.Categoria.Remover(categoriaDb);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoría borrada exitosamente" });
        }
        #endregion
    }
}
