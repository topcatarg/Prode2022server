namespace Prode2022Server.Models.UserForecast;

public class Match
{
    public int Id { get; set; }
    public string Team1Name { get; set; } = "";
    public string Team2Name { get; set; } = "";
    public string Team1Flag { get; set; } = "";
    public string Team2Flag { get; set; } = "";

    public int Team1Goals { get; set; }
    public int Team2Goals { get; set; }

    public bool Closed { get; set; }

    public string MyTeamName { get; set; } = "";

    public int MyTeamId { get; set; }

    public string StageName { get; set; } = "";
    public string Date { get; set; } = "";

}