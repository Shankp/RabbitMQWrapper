using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Configuration;
using System.Text;
using System.Threading;

namespace TestRabbitMQApp
{
    class Program
    {
        private static IModel m_model;
        private static IConnection m_connection;
        private static Action<string> m_callback;
        private static string queueName = "test-queue";

        static void Main(string[] args)
        {           

            var consumerTag = SubscriberService.Subscriber(queueName);

            while (true)
            {                
                Console.Write("Enter the message : ");
                var message = Console.ReadLine();
                if (message.ToUpper().Equals("EXIT"))
                {
                    Console.Write("Stopping the system.");
                    break;
                }
                PublisherService.Publisher(queueName, message);
               
                Thread.Sleep(1000);
                Console.WriteLine(Environment.NewLine);
            } 
        }


    }


}
