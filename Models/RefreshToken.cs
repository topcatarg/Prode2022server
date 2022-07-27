namespace Prode2022Server.Models;

public partial class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = "";
    public DateTime ExpiryDate { get; set; }

    //public virtual User User { get; set; }
}