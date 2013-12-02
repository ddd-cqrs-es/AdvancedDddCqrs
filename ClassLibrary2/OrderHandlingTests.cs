﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClassLibrary2
{
    [TestFixture]
    public class OrderHandlingTests
    {
        [Test]
        public void WaiterHandlesOrder()
        {
            var orderHandler = new TestableOrderHandler();
            var waiter = new Waiter(orderHandler);
            var tableNumber = 12;

            waiter.TakeOrder(tableNumber, new OrderItem[0], Guid.NewGuid());

            Assert.That(orderHandler.Order, Is.Not.Null);
            Assert.That(orderHandler.Order.TableNumber, Is.EqualTo(tableNumber));
        }

        [Test]
        public void CookHandlesOrder()
        {
            var testableOrderHandler = new TestableOrderHandler();
            var waiter = new Waiter(new Cook(testableOrderHandler));
            waiter.TakeOrder(
                12,
                new[]
                {
                    new OrderItem
                    {
                        Name = "Beans on Toast",
                        Quantity = 1
                    }
                },
                Guid.NewGuid());
            
            Assert.That(testableOrderHandler.Order, Is.Not.Null);
        }

        [Test]
        public void AssManHandlesOrder()
        {
            var testableOrderHandler = new TestableOrderHandler();
            var waiter = new Waiter(new Cook(new AssMan(testableOrderHandler)));

            waiter.TakeOrder(
               12,
               new[]
                {
                    new OrderItem
                    {
                        Name = "Beans on Toast",
                        Quantity = 1
                    }
                },
                Guid.NewGuid());

            Assert.That(testableOrderHandler.Order, Is.Not.Null);

        }

        [Test]
        public void CashierHandlesOrder()
        {
            var testableOrderHandler = new TestableOrderHandler();
            var cashier = new Cashier(testableOrderHandler);
            var waiter = new Waiter(new Cook(new AssMan(cashier)));
            var orderId = Guid.NewGuid();

            waiter.TakeOrder(
               12,
               new[]
                {
                    new OrderItem
                    {
                        Name = "Beans on Toast",
                        Quantity = 1
                    }
                },
                orderId
                );

            cashier.TryPay(orderId);

            Assert.That(testableOrderHandler.Order, Is.Not.Null);
            Assert.That(testableOrderHandler.Order.IsPaid, Is.True);
        }

        [Test]
        public void MultithreadedCookHandlesOrder()
        {
            var testableOrderHandler = new TestableOrderHandler();
            var cashier = new Cashier(testableOrderHandler);
            var waiter = new Waiter(new BlockingCollectionAsyncHandler(new Cook(new AssMan(cashier))));
            var orderId = Guid.NewGuid();

            waiter.TakeOrder(
               12,
               new[]
                {
                    new OrderItem
                    {
                        Name = "Beans on Toast",
                        Quantity = 1
                    }
                },
                orderId
                );

            while (cashier.TryPay(orderId) == false)
            {
                Console.WriteLine("Couldn't find order {0}, trying again...", orderId);
                Thread.Sleep(500);
            }

            Assert.That(testableOrderHandler.Order, Is.Not.Null);
            Assert.That(testableOrderHandler.Order.IsPaid, Is.True);
        }

        [Test]
        public void MultithreadedTeamHandlesLotsOfOrders()
        {
            var cwHandler = new TestableOrderHandler();
            var cashierInner = new Cashier(cwHandler);
            var cashier = new BlockingCollectionAsyncHandler(cashierInner);
            var assMan = new BlockingCollectionAsyncHandler(new AssMan(cashier));
            var cook = new BlockingCollectionAsyncHandler(new Cook(assMan));
            var waiter = new Waiter(cook);

            var writer = Task.Factory.StartNew(() =>
            {
                while(true)
                {
                    Console.WriteLine("Cook Queue Length:    {0}", cook.QueueLength);
                    Console.WriteLine("AssMan Queue Length:  {0}", assMan.QueueLength);
                    Console.WriteLine("Cashier Queue Length: {0}", cashier.QueueLength);
                    Thread.Sleep(1000);
                }
            },
            TaskCreationOptions.AttachedToParent);

            var orderCount = 200;

            BlockingCollection<Guid> orderIds = new BlockingCollection<Guid>();

            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < orderCount; i++)
                {
                    var orderId = Guid.NewGuid();
                    waiter.TakeOrder(
                        12,
                        new[]
                        {
                            new OrderItem
                            {
                                Name = "Beans on Toast",
                                Quantity = 1
                            }
                        },
                        orderId
                        );

                    orderIds.Add(orderId);
                }

                orderIds.CompleteAdding();
            });

            var waitHandle = new ManualResetEvent(false);
            Task.Factory.StartNew(() =>
            {
                foreach (var orderId in orderIds.GetConsumingEnumerable())
                {
                    while (cashierInner.TryPay(orderId) == false)
                    {
                        Thread.Sleep(1);
                    }
                }

                waitHandle.Set();
            });

            waitHandle.WaitOne();
        }
    }
}