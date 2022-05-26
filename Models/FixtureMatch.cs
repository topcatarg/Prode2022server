using System.ComponentModel.DataAnnotations;

namespace Prode2022Server.Models
{
    public class FixtureMatch
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Ingrese la fecha del partido")]
        public string? Date { get; set; }
        [Required(ErrorMessage = "Ingrese la hora del partido")]
        public string? Time { get; set; }
        [Required(ErrorMessage = "Ingrese el equipo 1")]
        [UnlikeAttribute("Team2")]
        public int? Team1 { get; set; } = null;
        [Required(ErrorMessage = "Ingrese el equipo 2")]
        [UnlikeAttribute("Team1")]
        public int? Team2 { get; set; } = null;
        [Required(ErrorMessage = "Ingrese el grupo")]

        public int? Stage { get; set; } = null;

        public string? StageName { get; set; }
        public string? Team1Name { get; set; }
        public string? Team2Name { get; set; }
        public string FormatedDate {
            get
            {
                return $"{Date} {Time}";
            }
        }
        public string? Team1Flag {get;set;}
        public string? Team2Flag {get;set;}
    }
}