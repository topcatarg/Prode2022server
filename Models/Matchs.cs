namespace Prode2022Server.Models
{
    public class Matchs 
    {
        public int Id { get; set; }
        public DateTime Date{get; set;}
        public string? StandardDate { get; set; }
        public int Team1 {get; set;}
        public int Team2 {get; set;}

        public string? StageName {get; set;}

        public string? Team1Name {get;set;}
        public string? Team2Name {get;set;}

        public string? Team1Flag {get;set;}
        public string? Team2Flag {get;set;}
    }
}