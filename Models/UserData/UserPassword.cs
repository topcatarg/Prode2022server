namespace Prode2022Server.Models.UserData;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

public class UserPassword
{

    public int Id { get; set; }

    [Required(ErrorMessage = "Ingrese su password")]
    [StringLength(50, ErrorMessage = "El password debe contener entre 8 y 50 caracteres", MinimumLength = 8)]
    public string? Password { get; set; }
    [Required(ErrorMessage = "repita su password")]
    [Compare("Password", ErrorMessage = "Los password ingresados no son iguales")]
    public string? RepeatPassword { get; set; }
}
