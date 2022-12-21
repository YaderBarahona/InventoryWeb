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
    public class UsuarioAplicacionRepositorio : Repositorio<UsuarioAplicacion>,
                                                IUsuarioAplicacionRepositorio
    {
        private readonly ApplicationDbContext _db;
        public UsuarioAplicacionRepositorio(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
    }
}
