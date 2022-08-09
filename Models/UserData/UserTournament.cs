namespace Prode2022Server.Models.UserData;
using Prode2022Server.Helpers;
using System.ComponentModel.DataAnnotations;
public class UserTournament
{
    public string? TournamentName { get; set; }
    [Required(ErrorMessage = "No puede estar vacio")]
    [StringLength(50,ErrorMessage = "Debe agregar el nombre del equipo")]
    public string? TeamName { get; set; }
    public int Id{ get; set; }
    public bool HasPassword {
        get
        {
            return !Password.IsNullOrEmpty();
        }
    }

    public string? Password { get; set; }
    public int TournamentId{ get; set; }
    public string? UserPassword { get; set; }
    public int UserId { get; set; }
    public int UserTeamId { get; set; }


}