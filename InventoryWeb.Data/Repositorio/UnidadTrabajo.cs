using InventoryWeb.Data.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace InventoryWeb.Data.Repositorio
{
    public class UnidadTrabajo : IUnidadTrabajo
    {
        private readonly ApplicationDbContext _db;
        public IBodegaRepositorio Bodega { get; private set; }
        public ICategoriaRepositorio Categoria { get; set; }
        public IMarcaRepositorio Marca { get; set; }

        public IProductoRepositorio Producto {get; set; }

        public IUsuarioAplicacionRepositorio UsuarioAplicacion {get; private set; }
        public ICompaniaRepositorio Compania { get; private set; }
        public ICarroComprasRepositorio CarroCompras { get; private set; }
        public IOrdenRepositorio Orden { get; private set; }
        public IOrdenDetalleRepositorio OrdenDetalle { get; private set; }
        public UnidadTrabajo(ApplicationDbContext db)
        {
            _db = db;
            Bodega = new BodegaRepositorio(_db); //inicializamos unidad Bodega
            Categoria = new CategoriaRepositorio(_db); //inicializamos Categoria
            Marca = new MarcaRepositorio(_db); //inicializamos Marca
            Producto = new ProductoRepositorio(_db); //inicializamos Producto
            UsuarioAplicacion = new UsuarioAplicacionRepositorio(_db);
            Compania = new CompaniaRepositorio(_db);
            CarroCompras = new CarroComprasRepositorio(_db);
            Orden = new OrdenRepositorio(_db);
            OrdenDetalle = new OrdenDetalleRepositorio(_db);
        }
        public void Guardar()
        {
            _db.SaveChanges();
        }
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
