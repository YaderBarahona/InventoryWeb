using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Models
{
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }
        [Required]
        [Display(Name = "Nombre: ")]
        public String Nombre { get; set; }
        [Required]
        [Display(Name = "Descripción: ")]
        public String Descripcion { get; set; }
        [Required]
        //[Range(0, 9999.99, ErrorMessage = "Ingrese 0 hasta 9999.99")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Monto costo inicial: ")]
        public decimal Costo { get; set; }
        [Required]
        //[Range(0.00, 9999.99, ErrorMessage = "Ingrese 0 hasta 9999.99")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Monto precio inicial: ")]
        public decimal Precio { get; set; }
        [Required]
        [Range(0.00, 50000, ErrorMessage = "Ingrese 0 hasta 50000")]
        [Display(Name = "Existencia inicial: ")]
        public int Existencia { get; set; }
        [Required]
        [Display(Name = "Foto: ")]
        public byte[] Foto { get; set; }

        [ForeignKey("CategoriaId")]
        public int CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }
        [Required]
        public int MarcaId { get; set; }
        [ForeignKey("MarcaId")]
        public Marca Marca { get; set; }
    }
}
