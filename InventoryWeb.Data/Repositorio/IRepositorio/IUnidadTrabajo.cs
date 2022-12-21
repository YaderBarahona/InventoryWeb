using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryWeb.Data.Repositorio.IRepositorio
{
    public interface IUnidadTrabajo : IDisposable
    {
        //envolvemos todo lo necesario de todas las entidades del proyecto
        //wrapper
        IBodegaRepositorio Bodega { get; }
        ICategoriaRepositorio Categoria { get; }
        IMarcaRepositorio Marca { get; }
        IProductoRepositorio Producto { get; }
        IUsuarioAplicacionRepositorio UsuarioAplicacion { get; } 
        ICompaniaRepositorio Compania { get;  }
        ICarroComprasRepositorio CarroCompras { get; }
        IOrdenRepositorio Orden { get; }
        IOrdenDetalleRepositorio OrdenDetalle { get; }
        void Guardar();
    }
}
