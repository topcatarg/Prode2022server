namespace Prode2022Server.Models.UserData;

using System.ComponentModel.DataAnnotations;

public class UserEmail
{
    public int Id { get; set; }

    public string OriginalEmail { get; set; } = "";
    private string _Email = "";
    [Required(ErrorMessage = "Ingrese su email")]
    [EmailAddress(ErrorMessage = "Ingrese un email valido")]
    public string Email {
        get
        {
            return _Email;
        } 
        set
        {
            if (_Email != value)
            {
                _Email = value;
            }
            EmailChanged = _Email != OriginalEmail;
        } 
    }

    public bool EmailChanged { get; set; } = false;
}
