using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.models
{
    [Table("Categoria")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage="Este campo é obrigatorio")]
        [MaxLength(60, ErrorMessage="Este campo deve conter no maximo 60 caracteres")]
        [MinLength(3, ErrorMessage="Este campo deve conter no minimo 3 caracteres")]
        public string Title { get; set; }
    }
}