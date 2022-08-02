using System.ComponentModel.DataAnnotations;
using Prode2022Server.Helpers;

namespace Prode2022Server.Models
{
    public class Tournament
    {

        [Required(ErrorMessage = "Ingrese el nombre del torneo")]
        public string? Name { get; set; }
        public string? Password { get; set; }

        public int Id { get; set; }

        public int AdministratorId{ get; set; }
    }
}