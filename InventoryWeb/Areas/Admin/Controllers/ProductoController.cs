using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Models;
using Inventory.Models.ViewModels;
using InventoryWeb.Data;
using InventoryWeb.Data.Repositorio.IRepositorio;

namespace InventoryWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductoController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnidadTrabajo _unidadTrabajo;
        public ProductoController(ApplicationDbContext db, IUnidadTrabajo unidadTrabajo)
        {
            _db = db;
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public Microsoft.AspNetCore.Mvc.IActionResult Create()
        {
            CategoriaProductoVM model = new CategoriaProductoVM()
            {
                //Llena el combobox de compañías
                Producto = new Producto(),
                ListaCategoria =
          _unidadTrabajo.Categoria.ObtenerTodos().Select(c => new SelectListItem
          {
              Text = c.Nombre,
              Value = c.CategoriaId.ToString()
          }),
                ListaMarca =
                _unidadTrabajo.Marca.ObtenerTodos().Select(m => new SelectListItem
                {
                    Text = m.Nombre,
                    Value = m.MarcaId.ToString()
                }),
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaProductoVM models)
        {
            if (models != null)
            {
                var files = HttpContext.Request.Form.Files;
                Producto producto = new Producto();
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    producto.Nombre = models.Producto.Nombre;
                    producto.Descripcion = models.Producto.Descripcion;
                    producto.CategoriaId = models.Producto.CategoriaId;
                    producto.Existencia = models.Producto.Existencia;
                    producto.Costo = models.Producto.Costo;
                    producto.Precio = models.Producto.Precio;
                    producto.Foto = p1;
                    models.Producto.Foto = p1;
                }
                _db.Productos.Add(models.Producto);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(models);
            }
        }

        //GET Editar Producto
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _db.Productos.SingleOrDefaultAsync(m =>
                                 m.ProductoId == id);
            //ViewBag.Foto = producto.Foto;
            if (producto == null)
            {
                return NotFound();
            }

            CategoriaProductoVM model = new CategoriaProductoVM()
            {
  
                Producto = new Producto(),
                ListaCategoria =
                    _unidadTrabajo.Categoria.ObtenerTodos().Select(c => 
                    new SelectListItem
                    {
                      Text = c.Nombre,
                      Value = c.CategoriaId.ToString()
                   }),
               ListaMarca =
                    _unidadTrabajo.Marca.ObtenerTodos().Select(m => 
                    new SelectListItem {
                        Text = m.Nombre,
                        Value = m.MarcaId.ToString()
                  }),
            };
            model.Producto.ProductoId = producto.ProductoId;
            model.Producto.Nombre = producto.Nombre;
            model.Producto.Precio = producto.Precio;
            model.Producto.Costo = producto.Costo;
            model.Producto.Descripcion = producto.Descripcion;
            model.Producto.Existencia = producto.Existencia;
            model.Producto.Foto = producto.Foto;
            model.Producto.CategoriaId = producto.CategoriaId;
            model.Producto.MarcaId = producto.MarcaId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Producto producto)
        {
            if (producto.ProductoId == 0)
            {
                return NotFound();
            }

            var productoFromDb = await _db.Productos.Where(
                p => p.ProductoId == producto.ProductoId).FirstOrDefaultAsync();

            if (productoFromDb.ProductoId > 0)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    producto.Foto = p1;
                    productoFromDb.Foto = p1;
                }
                productoFromDb.Nombre = producto.Nombre;
                productoFromDb.Descripcion = producto.Descripcion;
                productoFromDb.CategoriaId = producto.CategoriaId;
                productoFromDb.MarcaId = producto.MarcaId;
                productoFromDb.Existencia = producto.Existencia;
                productoFromDb.Costo = producto.Costo;
                productoFromDb.Precio = producto.Precio;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _db.Productos.SingleOrDefaultAsync(m =>
                                 m.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }

            var categoria = await _db.Categorias.SingleOrDefaultAsync(c =>
                            c.CategoriaId == producto.CategoriaId);
            ViewBag.Categoria = categoria.Nombre;

            var marca = await _db.Marcas.SingleOrDefaultAsync(m =>
                            m.MarcaId == producto.MarcaId);
            ViewBag.Marca = marca.Nombre;

            return View(producto);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _db.Productos.SingleOrDefaultAsync(m => m.ProductoId == id);
            _db.Productos.Remove(producto);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var productos = await
           _db.Productos.Include(c => c.Categoria).Include(m => m.Marca).ToListAsync();
            return Json(new { data = productos });
        }
    }
}

