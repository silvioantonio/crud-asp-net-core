using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.models
{
    [Table("Produto")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage="Este campo é obrigatorio")]
        [MaxLength(60, ErrorMessage="Este campo deve conter no maximo 60 caracteres")]
        [MinLength(3, ErrorMessage="Este campo deve conter no minimo 3 caracteres")]
        public string Title { get; set; }

        [MaxLength(1024, ErrorMessage="Este campo deve conter no maximo 1024 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage="Este campo é obrigatorio")]
        [Range(1, int.MaxValue, ErrorMessage="O preço deve ser maior que 1")]
        [Column(TypeName = "decimal(7,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage="Este campo é obrigatorio")]
        [Range(1, int.MaxValue, ErrorMessage="Categoria invalida")]
        public int CategoryId { get; set; }

        public Category Category {get; set;}

    }
}