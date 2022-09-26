namespace Prode2022Server.Models.UserData;

using System.ComponentModel.DataAnnotations;

public class UserName
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ingrese su nombre de usuario")]
    public string Name { get; set; } = "";
}