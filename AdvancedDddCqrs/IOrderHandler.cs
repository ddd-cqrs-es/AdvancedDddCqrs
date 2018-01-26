namespace AdvancedDddCqrs
{
    public interface IOrderHandler
    {
        bool Handle(Order order);
    }
}
