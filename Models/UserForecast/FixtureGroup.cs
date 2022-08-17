namespace Prode2022Server.Models.UserForecast;

public class FixtureGroup
{
    public int StageId { get; set; }
    public string StageName { get; set; } = "";

    public bool Closed { get; set; } = false;

    public int ClosedStageId => StageId * -1;

}