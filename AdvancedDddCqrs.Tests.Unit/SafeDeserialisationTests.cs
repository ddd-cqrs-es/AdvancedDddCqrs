using System;

using NUnit.Framework;

namespace AdvancedDddCqrs.Tests.Unit
{
    [TestFixture]
    public class SafeDeserialisationTests
    {
        private readonly Guid _orderId = Guid.Parse("3DE16091-70B9-42D0-8F17-9FCED4B0F88A");

        [Test]
        public void Serialisation_ShouldWriteAllValues()
        {
            var serialiser = new MementoSerialiser();
            var order = new Order(12, _orderId, "Neil", false);
            order.AddItem(
                new OrderItem
                {
                    Cost = 2.99,
                    Name = "Beans on Toast",
                    IngredientsUsed = new string[0],
                    Quantity = 1
                });

            string json = serialiser.Serialise<Order, OrderMemento>(order);

            Assert.That(json, Is.EqualTo(GetJson()));
        }

        [Test]
        public void Deserialisation_ShouldNotReturnNull()
        {
            var serialiser = new Serialiser();
            string json = GetJson();
            var memento = serialiser.Deserialise<OrderMemento>(json);
            var order = new Order(memento);

            Assert.That(order, Is.Not.Null);
            Assert.That(order.ServerId, Is.EqualTo("Neil"));
        }

        [Test]
        public void RoundTripWithExtendedData()
        {
            var serialiser = new Serialiser();
            string initialJson = GetExtendedJson();
            var memento = serialiser.Deserialise<OrderMemento>(initialJson);
            var order = new Order(memento);

            string newJson = serialiser.Serialise(order.GetMemento());

            Assert.That(newJson, Is.EqualTo(initialJson));
        }

        private string GetJson()
        {
            return @"{
  ""Id"": """ + _orderId + @""",
  ""IsPaid"": false,
  ""Items"": [
    {
      ""Cost"": 2.99,
      ""Name"": ""Beans on Toast"",
      ""IngredientsUsed"": [],
      ""Quantity"": 1
    }
  ],
  ""ServerId"": ""Neil"",
  ""TableNumber"": 12
}";
        }

        private string GetExtendedJson()
        {
            return @"{
  ""Id"": """ + _orderId + @""",
  ""IsPaid"": false,
  ""Items"": [
    {
      ""Cost"": 2.99,
      ""Name"": ""Beans on Toast"",
      ""IngredientsUsed"": [],
      ""Quantity"": 1
    }
  ],
  ""ServerId"": ""Neil"",
  ""TableNumber"": 12,
  ""AwkwardCustomer"": true
}";
        }
    }
}
