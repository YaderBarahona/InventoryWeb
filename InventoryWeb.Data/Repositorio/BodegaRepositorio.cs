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
    public class BodegaRepositorio : Repositorio<Bodega>, IBodegaRepositorio
    {
        private readonly ApplicationDbContext _db;
        public BodegaRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Bodega bodega)
        {
            var bodegaDb = _db.Bodegas.FirstOrDefault(b => b.id == bodega.id);
            if (bodegaDb != null)
            {
                bodegaDb.Nombre = bodega.Nombre;
                bodegaDb.Descripcion = bodega.Descripcion;
                bodegaDb.Estado = bodega.Estado;
            }
        }
    }
}
