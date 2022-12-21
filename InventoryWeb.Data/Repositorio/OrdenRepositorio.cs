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
   public class OrdenRepositorio : Repositorio<Orden>,
                                   IOrdenRepositorio
    {
        private readonly ApplicationDbContext _db;
        public OrdenRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Actualizar(Orden orden)
        {
            _db.Update(orden);
        }
    }
}
