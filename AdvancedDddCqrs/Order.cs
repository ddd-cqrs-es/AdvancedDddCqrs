using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedDddCqrs
{
    public class Order : ISupportMemoisation<OrderMemento>
    {
        private readonly List<OrderItem> _items;

        public Order(OrderMemento memento)
        {
            TableNumber = memento.TableNumber;
            Id = memento.Id;
            ServerId = memento.ServerId;
            IsPaid = memento.IsPaid;
            DodgeyCustomer = memento.DodgeyCustomer;
            _items = new List<OrderItem>(memento.Items);
        }

        public Order(int tableNumber, Guid id, string serverId, bool dodgeyCustomer)
        {
            Id = id;
            TableNumber = tableNumber;
            ServerId = serverId;
            _items = new List<OrderItem>();
            DodgeyCustomer = dodgeyCustomer;
        }

        public int TableNumber { get; private set; }

        public Guid Id { get; }

        public string ServerId { get; }

        public bool IsPaid { get; private set; }

        public bool DodgeyCustomer { get; }

        public IEnumerable<OrderItem> Items => _items;

        public OrderMemento GetMemento()
        {
            return new OrderMemento
            {
                Id = Id,
                IsPaid = IsPaid,
                ServerId = ServerId,
                TableNumber = TableNumber,
                DodgeyCustomer = DodgeyCustomer,
                Items = Items.Select(x => new OrderItem
                {
                    Cost = x.Cost,
                    Name = x.Name,
                    Quantity = x.Quantity,
                    IngredientsUsed = x.IngredientsUsed.ToList()
                }).ToArray()
            };
        }

        public Order Clone()
        {
            return new Order(GetMemento());
        }

        public void AddItem(OrderItem item)
        {
            _items.Add(item);
        }

        public void SettleBill()
        {
            IsPaid = true;
        }

        public void MoveTable(int newTableNumber)
        {
            TableNumber = newTableNumber;
        }
    }
}
