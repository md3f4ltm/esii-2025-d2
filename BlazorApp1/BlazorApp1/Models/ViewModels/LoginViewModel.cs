using System.ComponentModel.DataAnnotations;

namespace ESII2025d2.Models.ViewModels;

public class LoginViewModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor introduza o seu e-mail")]
    [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
    public string? Email { get; set; }
    
    [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor introduza a sua palavra-passe")]
    public string? Password { get; set; }
    
}