namespace AdvancedDddCqrs
{
    public interface IHandler<T>
    {
        bool Handle(T message);
    }
}
