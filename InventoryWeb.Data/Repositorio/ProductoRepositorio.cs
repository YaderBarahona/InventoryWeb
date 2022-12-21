using Inventory.Models;
using InventoryWeb.AccesoDatos.Repositorio;
using InventoryWeb.Data.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 
namespace InventoryWeb.Data.Repositorio
{
    class ProductoRepositorio : Repositorio<Producto>,
                                IProductoRepositorio
    {
        private readonly ApplicationDbContext _db;
        public ProductoRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Actualizar(Producto producto)
        {
            var productoDb =
       _db.Productos.FirstOrDefault(p =>
           p.ProductoId == producto.ProductoId);
            if (productoDb != null)
            {
                productoDb.Nombre = producto.Nombre;
                productoDb.Descripcion = producto.Descripcion;
                productoDb.CategoriaId = producto.CategoriaId;
                productoDb.Precio = producto.Precio;
                productoDb.Costo = producto.Costo;
                productoDb.Foto = producto.Foto;
                productoDb.Existencia = producto.Existencia;
            }
        }
    }
}
