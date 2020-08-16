using System;
using System.Threading.Tasks;
using MassTransit;

namespace SendVsPublish
{
    class Program
    {
        static async Task Main(string[] args)
        {
            EndpointConvention.Map<MyEvent>(new Uri("rabbitmq://localhost:5672/MyEventQueue1-2"));
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("MyEventQueue1-2", e =>
                {
                    e.Consumer<MyEventConsumer1>();
                    e.Consumer<MyEventConsumer2>();
                });
                cfg.ReceiveEndpoint("MyEventQueue3", e =>
                {
                    e.Consumer<MyEventConsumer3>();
                });
            });

            await busControl.StartAsync();

            do
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.S)
                {
                    await busControl.Send(new MyEvent());
                }
                else if (key.Key == ConsoleKey.P)
                {
                    await busControl.Publish(new MyEvent());
                }
                else
                {
                    break;
                }
            } while (true);
        }
    }

    public class MyEventConsumer1 : IConsumer<MyEvent>
    {
        public Task Consume(ConsumeContext<MyEvent> context)
        {
            Console.WriteLine("1 MyEvent received");

            return Task.CompletedTask;
        }
    }

    public class MyEventConsumer2 : IConsumer<MyEvent>
    {
        public Task Consume(ConsumeContext<MyEvent> context)
        {
            Console.WriteLine("2 MyEvent received");

            return Task.CompletedTask;
        }
    }

    public class MyEventConsumer3 : IConsumer<MyEvent>
    {
        public Task Consume(ConsumeContext<MyEvent> context)
        {
            Console.WriteLine("3 MyEvent received");

            return Task.CompletedTask;
        }
    }

    public class MyEvent
    {
    }
}
