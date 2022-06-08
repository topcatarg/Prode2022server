using System.ComponentModel.DataAnnotations;
using Prode2022Server.Helpers;

namespace Prode2022Server.Models
{
    public class UserLogin
    {

        [Required(ErrorMessage = "Ingrese su email")]
        [EmailAddress(ErrorMessage = "Ingrese un email valido")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Ingrese su password")]
        public string? Password { get; set; }
        
    }
}