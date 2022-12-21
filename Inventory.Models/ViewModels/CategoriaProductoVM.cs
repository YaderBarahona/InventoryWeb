using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace Inventory.Models.ViewModels
{
    public class CategoriaProductoVM
    {
        public IEnumerable<SelectListItem> ListaCategoria { get; set; }
        public IEnumerable<SelectListItem> ListaMarca { get; set; }
        public IEnumerable<SelectListItem> ListaBodega { get; set; }

        public Producto Producto { get; set; }
    }
}
