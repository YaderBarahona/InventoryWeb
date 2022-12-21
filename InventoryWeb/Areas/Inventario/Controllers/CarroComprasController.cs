using Inventory.Models.ViewModels;
using Inventory.Models;
using InventoryWeb.Data.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using InventoryWeb.Utilities;
using Stripe;
using InventoryWeb.Data;
using Rotativa.AspNetCore;

namespace InventoryWeb.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class CarroComprasController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public CarroComprasVM CarroComprasVM { get; set; }
        public CarroComprasController(IUnidadTrabajo unidadTrabajo,
                                      IEmailSender emailSender,
                                      UserManager<IdentityUser> userManager,
                                      ApplicationDbContext db)
        {
            _unidadTrabajo = unidadTrabajo;
            _emailSender = emailSender;
            _userManager = userManager;
            _db = db;
        }
        public IActionResult Index()
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);

            CarroComprasVM = new CarroComprasVM()
            {
                Orden = new Inventory.Models.Orden(),
                CarroComprasLista = _unidadTrabajo.CarroCompras.ObtenerTodos(u => u.UsuarioAplicacionId ==
                                                                claim.Value, IncluirPropiedades: "Producto")
            };

            CarroComprasVM.Orden.TotalOrden = 0;
            CarroComprasVM.Orden.UsuarioAplicacion = _unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(u => u.Id == claim.Value);

            foreach (var lista in CarroComprasVM.CarroComprasLista)
            {
                lista.Precio = (double)lista.Producto.Precio;
                CarroComprasVM.Orden.TotalOrden += (lista.Precio * lista.Cantidad);
            }

            return View(CarroComprasVM);
        }
        public ActionResult mas(int carroId)
        {
            var carroCompras = _unidadTrabajo.CarroCompras.ObtenerPrimero(
                               c => c.Id == carroId, IncluirPropiedades: "Producto");
            carroCompras.Cantidad += 1;
            _unidadTrabajo.Guardar();
            return RedirectToAction(nameof(Index));
        }
        public ActionResult menos(int carroId)
        {
            var carroCompras = _unidadTrabajo.CarroCompras.ObtenerPrimero(
                               c => c.Id == carroId, IncluirPropiedades: "Producto");
           
            if(carroCompras.Cantidad == 1)
            {
                var numeroProductos = _unidadTrabajo.CarroCompras.ObtenerTodos(u =>
                                      u.UsuarioAplicacionId == carroCompras.UsuarioAplicacionId).ToList().Count();
                _unidadTrabajo.CarroCompras.Remover(carroCompras);
                _unidadTrabajo.Guardar();
                HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos - 1);
            }
            else
            {
                carroCompras.Cantidad -= 1;
                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            carroCompras.Cantidad += 1;
            _unidadTrabajo.Guardar();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult remover(int carroId)
        {
            var carroCompras = _unidadTrabajo.CarroCompras.ObtenerPrimero(
                  c => c.Id == carroId, IncluirPropiedades: "Producto");
            var numeroProductos = _unidadTrabajo.CarroCompras.ObtenerTodos(
                u => u.UsuarioAplicacionId == carroCompras.UsuarioAplicacionId).ToList().Count();
            _unidadTrabajo.CarroCompras.Remover(carroCompras);
            _unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos - 1);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
      public IActionResult Proceder()
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);
            CarroComprasVM = new CarroComprasVM()
            {
                Orden = new Inventory.Models.Orden(),
                CarroComprasLista = _unidadTrabajo.CarroCompras.ObtenerTodos(u => u.UsuarioAplicacionId ==
                                                                claim.Value, IncluirPropiedades: "Producto"),
                //para controlar que se respete el stock de inventario de una bodega al momento de la venta
                Compania = _unidadTrabajo.Compania.ObtenerPrimero()
            };
            CarroComprasVM.Orden.TotalOrden = 0;
            CarroComprasVM.Orden.UsuarioAplicacion = _unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(u => u.Id == claim.Value);
            foreach (var lista in CarroComprasVM.CarroComprasLista)
            {
                lista.Precio = (double)lista.Producto.Precio;
                CarroComprasVM.Orden.TotalOrden += (lista.Precio * lista.Cantidad);
            }
            CarroComprasVM.Orden.NombreCliente = CarroComprasVM.Orden.UsuarioAplicacion.Nombres + " " +
                                                  CarroComprasVM.Orden.UsuarioAplicacion.Apellidos;
            CarroComprasVM.Orden.Telefono = CarroComprasVM.Orden.UsuarioAplicacion.Telefono;
            CarroComprasVM.Orden.Direccion = CarroComprasVM.Orden.UsuarioAplicacion.Direccion;
            CarroComprasVM.Orden.Pais = CarroComprasVM.Orden.UsuarioAplicacion.Pais;
            CarroComprasVM.Orden.Ciudad = CarroComprasVM.Orden.UsuarioAplicacion.Ciudad;
            //Controlar stock 
            foreach (var item in CarroComprasVM.CarroComprasLista)
            {
                //capturar el stock de cada producto
                //var producto = _db.BodegaProducto.FirstOrDefault(
                //                  b => b.ProductoId == item.ProductoId &&
                //                  b.BodegaId == CarroComprasVM.Compania.BodegaVentaId);
                var producto = _db.BodegaProducto.FirstOrDefault(
                  b => b.ProductoId == item.ProductoId);

                if (item.Cantidad > producto.Cantidad){
                    TempData["Error"] = " La cantidad del producto " + item.Producto.Descripcion
                                        + " excede al stock que es de " + producto.Cantidad;
                    //que se quede en el index y no vaya al Post
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(CarroComprasVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Proceder")]
        public IActionResult ProcederPost(string stripeToken)
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);
            CarroComprasVM.Orden.UsuarioAplicacion = _unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(c => c.Id == claim.Value);
            CarroComprasVM.CarroComprasLista = _unidadTrabajo.CarroCompras.ObtenerTodos(
                                                    c => c.UsuarioAplicacionId == claim.Value, IncluirPropiedades: "Producto");
            CarroComprasVM.Orden.EstadoOrden = DS.EstadoPendiente;
            CarroComprasVM.Orden.EstadoPago = DS.PagoEstadoPendiente;
            CarroComprasVM.Orden.UsuarioAplicacionId = claim.Value;
            CarroComprasVM.Orden.FechaOrden = DateTime.Now;

            //Para descarga del inventario
            CarroComprasVM.Compania = _unidadTrabajo.Compania.ObtenerPrimero();


            _unidadTrabajo.Orden.Agregar(CarroComprasVM.Orden);
            _unidadTrabajo.Guardar();

            foreach (var item in CarroComprasVM.CarroComprasLista)
            {
                OrdenDetalle ordenDetalle = new OrdenDetalle()
                {
                    ProductoId = item.ProductoId,
                    OrdenId = CarroComprasVM.Orden.Id,
                    Precio = (double)item.Producto.Precio,
                    Cantidad = item.Cantidad
                };
                CarroComprasVM.Orden.TotalOrden += ordenDetalle.Cantidad * ordenDetalle.Precio;
                _unidadTrabajo.OrdenDetalle.Agregar(ordenDetalle);

            }
            // Remover los productos del carro de Compras
            _unidadTrabajo.CarroCompras.RemoverRango(CarroComprasVM.CarroComprasLista);
            _unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(DS.ssCarroCompras, 0);

            if (stripeToken == null)
            {

            }
            else
            {
                //Procesar el Pago
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(CarroComprasVM.Orden.TotalOrden * 100),
                    Currency = "USD",
                    Description = "Numero de Orden: " + CarroComprasVM.Orden.Id,
                    Source = stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                {
                    CarroComprasVM.Orden.EstadoPago = DS.EstadoRechazado;
                }
                else
                {
                    CarroComprasVM.Orden.TransaccionId = charge.BalanceTransactionId;
                    //CarroComprasVM.Orden.TransaccionId = charge.Id;
                }
                if (charge.Status.ToLower() == "succeeded")
                {
                    CarroComprasVM.Orden.EstadoPago = DS.PagoEstadoAprobado;
                    CarroComprasVM.Orden.EstadoOrden = DS.EstadoAprobado;
                    CarroComprasVM.Orden.FechaPago = DateTime.Now;
                }
                //Se realiza el rebajo del stock del producto por la bodega asignada
                foreach (var item in CarroComprasVM.CarroComprasLista)
                {
                    var producto = _db.BodegaProducto.FirstOrDefault
                        (b => b.ProductoId == item.ProductoId);
                    producto.Cantidad -= item.Cantidad;
                }

            }
            _unidadTrabajo.Guardar();
            return RedirectToAction("OrdenConfirmacion", "CarroCompras", 
                new { id = CarroComprasVM.Orden.Id });
        }
        public IActionResult OrdenConfirmacion(int id)
        {
            return View(id);
        }
        public IActionResult ImprimirOrden(int id)
        {
            CarroComprasVM = new CarroComprasVM();
            CarroComprasVM.Compania = _unidadTrabajo.Compania.ObtenerPrimero();
            CarroComprasVM.Orden = _unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == id,
                IncluirPropiedades: "UsuarioAplicacion");
            CarroComprasVM.OrdenDetalleLista = _unidadTrabajo.OrdenDetalle.ObtenerTodos
                (d => d.OrdenId == id, IncluirPropiedades: "Producto");
            return new ViewAsPdf("ImprimirOrden", CarroComprasVM)
            {
                FileName = "Ordenes" + CarroComprasVM.Orden.Id + ".pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                CustomSwitches = "--page-offset 9 --footer-center [page] --footer-font-size 12"
            };
        }
 }
}
