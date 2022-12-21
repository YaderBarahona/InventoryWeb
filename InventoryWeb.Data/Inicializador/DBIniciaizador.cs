using Inventory.Models;
using InventoryWeb.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryWeb.Data.Inicializador
{
    public class DBInicializador : IDbInicializador
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DBInicializador(ApplicationDbContext db,
                              UserManager<IdentityUser> usermanager,
                              RoleManager<IdentityRole> roleManager)
                                
        {
            _db = db;
            _userManager = usermanager;
            _roleManager = roleManager;
        }
        public void Inicializar()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0) {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {
                throw;
            }
            if (_db.Roles.Any(r => r.Name == DS.Role_Admin)) return;
            _roleManager.CreateAsync(new IdentityRole(DS.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(DS.Role_Cliente)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(DS.Role_Auditor)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(DS.Role_Ventas)).GetAwaiter().GetResult();
            _userManager.CreateAsync(new UsuarioAplicacion
            {
                UserName = "administrador",
                Email = "enrique.gomezcr@gmail.com",
                EmailConfirmed = true,
                Nombres = "Enrique",
                Apellidos = "Gómez"
            }, "Admin#2022").GetAwaiter().GetResult();
            UsuarioAplicacion user = _db.UsuarioAplicacion.Where(u => u.UserName == "administrador").FirstOrDefault();
            _userManager.AddToRoleAsync(user, DS.Role_Admin).GetAwaiter().GetResult();
        }
    }
}
