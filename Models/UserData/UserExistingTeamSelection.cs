namespace Prode2022Server.Models.UserData;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

public class UserExistingTeamSelection
{
    public int SelectedTeam { get; set; } = 0;
    public ImmutableArray<UserTournament> TeamList { get; set; } = new();
    public string TournamentPassword { get; set; } = "";
    [Compare("TournamentPassword", ErrorMessage = "La contraseña no es correcta")]
    public string Password { get; set; } = "";
}

