using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace productService.Models
{
    public class Product
    {
        [Key]
        public int Id { set; get; }
        [Column(TypeName="nvarchar(100)"), MaxLength(100), Required]
        public string Name { set; get; }
        public int Price { set; get; }
        [MaxLength(53)]
        public double Amount { set; get; }
        //public DateTime CreatedTime { set; get; }
        public DateTime? RemovedTime { set; get; }
        [MaxLength(100)]
        public string Note { set; get; }
    }
}
