//ViewModels vem da Tela/Postman

using System.ComponentModel.DataAnnotations;

namespace Blogue.ViewModels.Categories;

public class EditorCategoryViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(40, MinimumLength = 3, 
        ErrorMessage = "Este campo deve conter no maximo 40 caracteres e no minimo 3 caracteres")]
    public string Name { get; set; }
    [Required(ErrorMessage = "O slug é obrigatório")]
    public string Slug { get; set; }
}