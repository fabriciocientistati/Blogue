using System.ComponentModel.DataAnnotations;

namespace Blogue.ViewModels.Accounts;

public class UploadImageViewModel
{
    [Required(ErrorMessage = "Imagem inválida")]
    public string Base64Image { get; set; }
}