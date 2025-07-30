namespace ReactivePrograming
{
    public interface IReadOnlyReactiveVar<T>
    {
        T Value { get; }
    }
}