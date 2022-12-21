using Inventory.Models;
using Inventory.Models.ViewModels;
using InventoryWeb.Data;
using InventoryWeb.Data.Repositorio.IRepositorio;
using InventoryWeb.Models;
using InventoryWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryWeb.Controllers
{
    [Area("Inventario")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, 
                              IUnidadTrabajo unidadTrabajo, ApplicationDbContext db)
        {
            _logger = logger;
            _unidadTrabajo = unidadTrabajo;
            _db = db;
        }
        [BindProperty]
        public CarroComprasVM CarroComprasVM { get; set; }
        [HttpGet]
        public IActionResult Index()
        {
            //IEnumerable<Producto> productoLista;
            //productoLista = _unidadTrabajo.Producto.ObtenerTodos(IncluirPropiedades: "Categoria,Marca");
            ProductoVM productoVM = new ProductoVM()
            {
                ProductoLista = _unidadTrabajo.Producto.ObtenerTodos(IncluirPropiedades: "Categoria,Marca"),
                CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.CategoriaId.ToString()
                })
            };
            
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var numeroProductos = _unidadTrabajo.CarroCompras.ObtenerTodos(
                       c => c.UsuarioAplicacionId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos);
            }
            return View(productoVM);
        }
        [HttpPost]
        public IActionResult Index(int categoria, string filtro)
        {
            ProductoVM productoVM = new ProductoVM();
            if (categoria == 0 && filtro == null)
            {
                productoVM = new ProductoVM()
                {
                    CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(c => new SelectListItem
                    {
                        Text = c.Nombre,
                        Value = c.CategoriaId.ToString()
                    }),
                    ProductoLista = _unidadTrabajo.Producto.ObtenerTodos(
                                     IncluirPropiedades: "Categoria,Marca")
                };

            }

            if (categoria > 0 && filtro == null)
            {
                productoVM = new ProductoVM()
                {
                    CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(c => new SelectListItem
                    {
                        Text = c.Nombre,
                        Value = c.CategoriaId.ToString()
                    }),
                ProductoLista = _unidadTrabajo.Producto.ObtenerTodos(IncluirPropiedades: "Categoria,Marca")
                    .Where(p => p.CategoriaId == categoria)
                };
            }

            if (categoria == 0 && filtro != null)
            {
                productoVM = new ProductoVM()
                {
                    CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(c => new SelectListItem
                    {
                        Text = c.Nombre,
                        Value = c.CategoriaId.ToString()
                    }),
                    ProductoLista = _unidadTrabajo.Producto.ObtenerTodos(IncluirPropiedades: "Categoria,Marca")
                    .Where(p => p.Descripcion.ToUpper().Contains(filtro.ToUpper()))
                };
            }
            if (categoria > 0 && filtro != null)
            {
                productoVM = new ProductoVM()
                {
                    CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(c => new SelectListItem
                    {
                        Text = c.Nombre,
                        Value = c.CategoriaId.ToString()
                    }),

                    ProductoLista = _unidadTrabajo.Producto.ObtenerTodos(IncluirPropiedades: "Categoria,Marca").
                                                   Where(p => p.CategoriaId == categoria && p.Descripcion.ToUpper().Contains(filtro.ToUpper()))
                };
            }

            return View(productoVM);
        }
        [HttpGet]
        public IActionResult Detalle(int id)
        {
            CarroComprasVM CarroComprasVM = new CarroComprasVM();
            //CarroComprasVM.Compania = _db.Compania.FirstOrDefault();
            CarroComprasVM.BodegaProducto = _db.BodegaProducto.Include(p => p.Producto)
                   .Include(p => p.Producto.Categoria).Include(p => p.Producto.Marca)
                   .FirstOrDefault(b => b.ProductoId == id);
            if (CarroComprasVM.BodegaProducto == null)
            {
                TempData["NoProducto"] = "No existe inventario para este producto";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["NoProducto"] = "";
                CarroComprasVM.CarroCompras = new CarroCompras()
                {
                    Producto = CarroComprasVM.BodegaProducto.Producto,
                    ProductoId = CarroComprasVM.BodegaProducto.ProductoId
                };
                return View(CarroComprasVM);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Detalle(CarroComprasVM carroComprasVM)
        {
            var claimIdentidad = (ClaimsIdentity) User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);
            carroComprasVM.CarroCompras.UsuarioAplicacionId = claim.Value;
            CarroCompras carroDb = _unidadTrabajo.CarroCompras.ObtenerPrimero(
                u => u.UsuarioAplicacionId == carroComprasVM.CarroCompras.UsuarioAplicacionId
                && u.ProductoId == carroComprasVM.CarroCompras.ProductoId,
                IncluirPropiedades: "Producto"
                );
            if (carroDb == null)
            {
                //El usuario conectado no tiene compras de productos en el carrito de compras
                _unidadTrabajo.CarroCompras.Agregar(carroComprasVM.CarroCompras);
            }
            else
            {
                carroDb.Cantidad += carroComprasVM.CarroCompras.Cantidad;
                _unidadTrabajo.CarroCompras.Actualizar(carroDb);
            }
            _unidadTrabajo.Guardar();
            //es el número de productos que tiene asignado el usuario en su carrito de compras, no cantidades.
            var numeroProductos = _unidadTrabajo.CarroCompras.ObtenerTodos(
                c => c.UsuarioAplicacionId == carroComprasVM.CarroCompras.UsuarioAplicacionId).ToList().Count();
            HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Inventory.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
