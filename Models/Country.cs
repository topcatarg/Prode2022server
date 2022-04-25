using System.ComponentModel.DataAnnotations;

namespace Prode2022Server.Models

{
    public class Country
    {

        public int ID {get; set;}

        [Required]
        [StringLength(50,ErrorMessage = "Debe agregar el nombre del equipo")]
        public string? Team {get; set;}

        [Required]
        [StringLength(2,ErrorMessage = "Debe agregar el codigo de la bandera")]
        public string? Code {get; set;}

    }
}