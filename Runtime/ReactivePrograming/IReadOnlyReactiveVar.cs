namespace ReactivePrograming
{
    public interface IReadOnlyVar<T>
    {
        T Value { get; }
    }
}