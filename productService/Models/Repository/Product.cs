using System;
using System.ComponentModel.DataAnnotations;

namespace productService.Models
{
    public class Product
    {
        [Key]
        public int Id { set; get; }
        [MaxLength(20)]
        [Required]
        public string Name { set; get; }
        public int Price { set; get; }
        //public DateTime CreatedTime { set; get; }
        public DateTime? RemovedTime { set; get; }
        [MaxLength(100)]
        public string Note { set; get; }
    }
}
