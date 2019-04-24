namespace XipeServices.Validations
{
    public interface IComparisonRule<T>
    {
        string ValidationMessage { get; set; }
        bool Compare(T value);
    }
}
