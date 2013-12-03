﻿using System;
using System.Collections.Generic;

namespace ClassLibrary2
{
   public interface IHaveTTL
   {
       bool HasExpired();
       void SetExpiry(TimeSpan duration);
   }

    public class Order : IHaveTTL
    {
        private DateTime _expiry;
        public int TableNumber { get; private set; }
        public IList<OrderItem> Items { get; set; }

        public Guid Id { get; set; }

        public bool IsPaid { get; set; }


        public Order(int tableNumber, Guid id)
        {
            Items = new List<OrderItem>();
            Id = id;
            TableNumber = tableNumber;
        }

        public void AddItem(OrderItem item)
        {
            Items.Add(item);
        }

        public bool HasExpired()
        {
            return DateTime.UtcNow > _expiry;
        }

        public void SetExpiry(TimeSpan duration)
        {
            _expiry = DateTime.UtcNow.Add(duration);
        }
    }
}