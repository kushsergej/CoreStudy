using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreStudy.Models
{
    public partial class Products
    {
        public Products()
        {
            OrderDetails = new HashSet<OrderDetails>();
        }

        public int ProductId { get; set; }

        [Required]
        [StringLength(40)]
        public string ProductName { get; set; }
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }

        [StringLength(20)]
        public string QuantityPerUnit { get; set; }

        [DataType(DataType.Currency)]
        [Range(1, 1000)]
        public decimal? UnitPrice { get; set; }

        [Range(1,100)]
        public short? UnitsInStock { get; set; }

        [Range(1, 100)]
        public short? UnitsOnOrder { get; set; }

        [Range(1, 100)]
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public Categories Category { get; set; }
        public Suppliers Supplier { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
