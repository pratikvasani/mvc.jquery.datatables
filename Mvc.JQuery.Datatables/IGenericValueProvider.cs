namespace Mvc.JQuery.DataTables
{
    public interface IGenericValueProvider
    {
        T GetValue<T>(string v);
    }
}