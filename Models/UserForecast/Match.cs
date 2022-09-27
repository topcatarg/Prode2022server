namespace Prode2022Server.Models.UserForecast;

public class Match
{
    public int Id { get; set; }
    public string Team1Name { get; set; } = "";
    public string Team2Name { get; set; } = "";
    public string Team1Flag { get; set; } = "";
    public string Team2Flag { get; set; } = "";

    private int team1Goals;
    public int Team1Goals 
    { 
        get
        {
            return team1Goals;
        }
        set
        {
            team1Goals = value;
            checkGoals();
        }
    }

    public int OriginalTeam1Goals { get; set; }
    private int team2Goals;
    public int Team2Goals 
    { 
        get
        {
            return team2Goals;
        }
        set
        {
            team2Goals = value;
            checkGoals();
        }
    }
    public int OriginalTeam2Goals { get; set; }

    private void checkGoals()
    {
        Modified = (team1Goals != OriginalTeam1Goals) || (team2Goals != OriginalTeam2Goals);
    }
    public bool Closed { get; set; }

    public string MyTeamName { get; set; } = "";

    public int MyTeamId { get; set; }

    public string StageName { get; set; } = "";
    public string Date { get; set; } = "";

    public bool Modified { get; set; } = false;

}