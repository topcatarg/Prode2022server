using System.ComponentModel.DataAnnotations;
using Prode2022Server.Helpers;

namespace Prode2022Server.Models
{
    public class NewUserData
    {
        [Required(ErrorMessage = "Ingrese su nombre de usuario")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Ingrese su email")]
        [EmailAddress(ErrorMessage = "Ingrese un email valido")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Ingrese su password")]
        [StringLength(50,ErrorMessage = "El password debe contener entre 8 y 50 caracteres",MinimumLength = 8)]
        public string? Password { get; set; }
        [Required(ErrorMessage = "repita su password")]
        [Compare("Password",ErrorMessage = "Los password ingresados no son iguales")]
        public string? RepeatPassword { get; set; }

    }
}