using System.ComponentModel.DataAnnotations;

namespace Prode2022Server.Models

{
    public class FixtureGroups
    {

        public int ID {get; set;}

        [Required]
        [StringLength(20,ErrorMessage = "Debe agregar el nombre del grupo")]
        public string? GroupName {get; set;}

    }
}