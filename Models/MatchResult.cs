namespace Prode2022Server.Models
{
    public class MatchResult
    {
        public int Id { get; set; }
        public string? Date { get; set; }
        public string? StageName { get; set; }

        public string? Team1Name { get; set; }
        public string? Team2Name { get; set; }

        public int Team1Goals { get; set; }
        public int Team2Goals { get; set; }
        public bool Closed { get; set; }

        /// <summary>
        /// Marks the class as editable for forms
        /// </summary>
        public bool Editable { get; set; } = false;
    }
}