public interface IGenericListNotifier<T> where T: class
{
    Task ChangeRow(T? c);
    Task ChangeData();
    event Func<Task>? NotifyList;
    event Func<Task>? NotifyDataControl;
    public T? data {get;set;}
}

public class GenericListNotifier<T>: IGenericListNotifier<T> where T: class
{
    public T? data {get;set;} = default(T);
    public event Func<Task>? NotifyList;
    public event Func<Task>? NotifyDataControl;

    public async Task ChangeRow(T? c)
    {
        if (c == null)
        {
            data = default(T);
        }
        else 
        {
            data = c;
        }
        if (NotifyDataControl != null)
            await NotifyDataControl.Invoke();
    }

    public async Task ChangeData()
    {
        if (NotifyList != null)
        {
            await NotifyList.Invoke();
        }
    }
}