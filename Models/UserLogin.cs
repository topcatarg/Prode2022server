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
        public bool LoggedIn { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsExpired { get; set; }
        public string? Name { get; set; }

        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public int Id { get; set; }
    }
}