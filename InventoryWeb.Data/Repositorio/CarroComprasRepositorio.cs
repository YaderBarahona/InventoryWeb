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
    public class CarroComprasRepositorio : Repositorio<CarroCompras>,
                                 ICarroComprasRepositorio
    {
        private readonly ApplicationDbContext _db;
        public CarroComprasRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Actualizar(CarroCompras carroCompras)
        {
            _db.Update(carroCompras);
        }
    }
}
