namespace AdvancedDddCqrs
{
    public class TestableOrderHandler : IOrderHandler
    {
        public Order Order { get; private set; }

        public bool Handle(Order order)
        {
            Order = order;
            ////Console.WriteLine(JsonConvert.SerializeObject(order, Formatting.Indented));

            return true;
        }
    }
}
