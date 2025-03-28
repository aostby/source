using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DapperGenericRepository.Model
{
    [Table("inv_product")]
    public class Example
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }
        [Column("product_name")]
        public string ProductName { get; set; }
        [Column("product_image")]
        public string ProductImage { get; set; }
        [Column("brand_id")]
        public int BrandId { get; set; }
        [Column("categories_id")]
        public int CategoriesId { get; set; }
        [Column("quantity")]
        public string Quantity { get; set; }
        [Column("rate")]
        public decimal Rate { get; set; }
        [Column("active")]
        public bool Active { get; set; }
        [Column("status")]
        public int Status { get; set; }
        [Column("unit")]
        public int Unit { get; set; }
    }
}