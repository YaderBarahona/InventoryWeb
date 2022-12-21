using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Models
{
    public class Inventario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Usuario")]
        public string UsuarioAplicacionId { get; set; }
        [ForeignKey("UsuarioAplicacionId")]
        public UsuarioAplicacion UsuarioAplicacion { get; set; }
        [Required]
        [Display(Name = "Fecha Inicial")]
        public DateTime FechaInicial { get; set; }
        [Display(Name = "Fecha Final")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:MM-dd-yyyy HH:mm}")]
        public DateTime FechaFinal { get; set; }
        [Required]
        [Display(Name = "Bodega")]
        public int BodegaId { get; set; }
        [ForeignKey("BodegaId")]
        public Bodega Bodega { get; set; }
        public bool Estado { get; set; }
    }
}
