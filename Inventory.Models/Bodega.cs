using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Models
{
    public class Bodega
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name = "Nombre: ")]
        public string Nombre { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Name = "Descripción: ")]
        public string Descripcion { get; set; }
        [Required]
        [Display(Name = "Estado: ")]
        public bool Estado { get; set; }
    }
}
