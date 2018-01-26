using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using AdvancedDddCqrs;
using AdvancedDddCqrs.Messages;

namespace ConsoleRunner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BackPressureTest();
        }

        private static void BackPressureTest()
        {
            var topicDispatcher = new TopicDispatcher();
            var threadBoundaryMonitor = new ThreadBoundaryMonitor();

            ThreadBoundary<Report> reporting = threadBoundaryMonitor.Wrap(new ReportingSystem(topicDispatcher));
            var cashierInner = new Cashier(topicDispatcher);
            ThreadBoundary<QueueOrderForPayment> cashier = threadBoundaryMonitor.Wrap(cashierInner);
            ThreadBoundary<PriceFood> assManager = threadBoundaryMonitor.Wrap(new AssistantManager(topicDispatcher));
            var cooks = new[]
            {
                threadBoundaryMonitor.Wrap(new Cook(topicDispatcher, 20)),
                threadBoundaryMonitor.Wrap(new Cook(topicDispatcher, 50)),
                threadBoundaryMonitor.Wrap(new Cook(topicDispatcher, 90))
            };
            TTLSettingHandler<CookFood> cookDispatcher =
                TTLSettingHandler.Wrap(
                    threadBoundaryMonitor.Wrap(
                        RetryDispatcher.Wrap(
                            TTLFilteringHandler.Wrap(
                                SmartDispatcher.Wrap(cooks, 15)))),
                    10);

            var waiter = new Waiter("Neil", topicDispatcher);

            topicDispatcher.Subscribe(cashier);
            topicDispatcher.Subscribe(cookDispatcher);
            topicDispatcher.Subscribe(assManager);
            topicDispatcher.Subscribe(reporting);
            topicDispatcher.Subscribe(threadBoundaryMonitor.Wrap(new Logger()));

            topicDispatcher.Subscribe(new SelfUnsubscribingOrderSampler(topicDispatcher));

            topicDispatcher.Subscribe(threadBoundaryMonitor.Wrap<OrderTaken>(new OrderFulfillmentCoordinator(topicDispatcher)));
            threadBoundaryMonitor.Start();

            RunTest(waiter, cashierInner, 5000);
        }

        private static void RunTest(Waiter waiter, Cashier cashier, int orderCount)
        {
            var orderIds = new BlockingCollection<Guid>();
            var ordersToBePaid = new BlockingCollection<Guid>();

            for (int i = 0; i < orderCount; i++)
            {
                orderIds.Add(Guid.NewGuid());
            }

            Task.Factory.StartNew(
                () =>
                {
                    bool isDodegy = false;

                    foreach (Guid orderId in orderIds)
                    {
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
                            orderId,
                            isDodegy
                        );
                        isDodegy = !isDodegy;
                        ordersToBePaid.Add(orderId);
                    }

                    //ordersToBePaid.CompleteAdding();
                });

            var waitHandle = new ManualResetEvent(false);
            Task.Factory.StartNew(
                () =>
                {
                    foreach (Guid orderId in ordersToBePaid.GetConsumingEnumerable())
                    {
                        if (false == cashier.TryPay(orderId))
                        {
                            ordersToBePaid.Add(orderId);
                        }
                    }

                    waitHandle.Set();
                });

            waitHandle.WaitOne();
            Console.ReadKey();
        }
    }
}
