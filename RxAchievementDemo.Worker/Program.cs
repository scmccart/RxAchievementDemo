using EasyNetQ;
using EasyNetQ.Topology;
using RxAchievementDemo.Models;
using System;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxAchievementDemo.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = RabbitHutch.CreateBus().Advanced;
            var exchange = Exchange.DeclareTopic("RCC_RX");
            var queue = Queue.DeclareDurable("Worker");
            queue.BindTo(exchange, "Task.#");

            var stream = new Subject<TodoEvent>();

            bus.Subscribe<TodoEvent>(queue, async (msg, info) =>
            {
                var todoEvent = msg.Body;
                Console.WriteLine("Recieved Message");

                stream.OnNext(todoEvent);

                Console.WriteLine("Pushed to Stream");
            });

            stream
                .ObserveOn(TaskPoolScheduler.Default)
                .Where(te => te.Type == EventType.Add)
                .Buffer(TimeSpan.FromSeconds(15))
                .Where(teList => teList.Count >= 3)
                .Do(teList => Console.WriteLine("Productive! {0}", teList.Count))
                .Subscribe(teList => NotifyClient("Productive!"));
        }

        static void NotifyClient(string message)
        {
            var client = new HttpClient();

            client.PostAsJsonAsync("http://localhost:45577/api/achievement", new { message }).Wait();
        }
    }
}
