using Prode2022Server.Models;

public class CountriesListNotifier
{
    public Country? country {get;set;} = null;
    public event Func<Task>? Notify;

    public async Task ChangeRow(Country? c)
    {
        if (c == null)
        {
            country = null;
        }
        else 
        {
            country = c;
        }
        if (Notify != null)
            await Notify.Invoke();
    }
}