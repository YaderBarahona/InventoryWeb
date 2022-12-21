using Inventory.Models;
using Inventory.Models.ViewModels;
using InventoryWeb.Data.Repositorio.IRepositorio;
using InventoryWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class CompaniaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CompaniaController(IUnidadTrabajo unidadTrabajo, 
             IWebHostEnvironment hostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _hostEnvironment = hostEnvironment;
        }


        public IActionResult Index()
        {
            var compania = _unidadTrabajo.Compania.ObtenerTodos();
            return View(compania);
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            CompaniaVM companiaVM = new CompaniaVM()
            {
                //Llena el combobox de compañías
                Compania = new Compania(),
                BodegaLista = 
                   _unidadTrabajo.Bodega.ObtenerTodos().Select(c => 
                           new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.id.ToString()
                }),
            };


            if (id == null)
            {
                //Esto es para Crear nuevo Registro
                return View(companiaVM);
            }
            //Esto es para Actualizar
            companiaVM.Compania = _unidadTrabajo.Compania.Obtener(id.GetValueOrDefault());
            if (companiaVM.Compania == null)
            {
                return NotFound();
            }

            return View(companiaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CompaniaVM companiaVM)
        {
            if (ModelState.IsValid)
            {

                // Cargar Imagenes
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    Compania companiaDB = _unidadTrabajo.Compania.Obtener(companiaVM.Compania.Id);
                    if (companiaDB != null)
                        companiaVM.Compania.LogoUrl = companiaDB.LogoUrl;


                    string filename = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"imagenes\companias");
                    var extension = Path.GetExtension(files[0].FileName);

                    if (companiaVM.Compania.LogoUrl != null)
                    {
                        //Esto es para editar, necesitamos borrar la imagen anterior
                        var imagenPath = Path.Combine(webRootPath, companiaVM.Compania.LogoUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagenPath))
                        {
                            System.IO.File.Delete(imagenPath);
                        }
                    }

                    using (var filesStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    companiaVM.Compania.LogoUrl = @"\imagenes\companias\" + filename + extension;
                }
                else
                {
                    // Si en el Update el usuario no cambia la imagen
                    if (companiaVM.Compania.Id != 0) 
                    {
                        Compania companiaDB = _unidadTrabajo.Compania.Obtener(companiaVM.Compania.Id);
                        companiaVM.Compania.LogoUrl = companiaDB.LogoUrl;
                    }
                }


                if (companiaVM.Compania.Id == 0)
                {
                    _unidadTrabajo.Compania.Agregar(companiaVM.Compania);
                }
                else
                {
                    _unidadTrabajo.Compania.Actualizar(companiaVM.Compania);
                }
                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                companiaVM.BodegaLista = _unidadTrabajo.Bodega.ObtenerTodos().Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.id.ToString()
                });


                if (companiaVM.Compania.Id != 0)
                {
                    companiaVM.Compania = _unidadTrabajo.Compania.Obtener(companiaVM.Compania.Id);
                }

            }
            return View(companiaVM.Compania);
        }

        #region API
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _unidadTrabajo.Compania.ObtenerTodos(IncluirPropiedades: "Bodega");
            return Json(new { data = todos });
        }
        #endregion
    }
}
