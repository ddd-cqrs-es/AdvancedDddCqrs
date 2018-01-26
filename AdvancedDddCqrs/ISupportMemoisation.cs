namespace AdvancedDddCqrs
{
    public interface ISupportMemoisation<T>
    {
        T GetMemento();
    }
}
