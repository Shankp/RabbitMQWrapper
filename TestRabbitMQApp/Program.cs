using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace TestRabbitMQApp
{
    class Program
    {
        private static IModel m_model;
        private static IConnection m_connection;
        private static Action<string> m_callback;
        static void Main(string[] args)
        {
            if (m_model == null || m_model.IsClosed)
                InitiateConnection();

            m_callback = TestCallBacmMethod;

            //Declaring exchange
            m_model.ExchangeDeclare("demoExchange", ExchangeType.Direct);

            //Declare queue
            m_model.QueueDeclare("test-queue", true, false, false, null);

            //Bind queue to the exchange
            m_model.QueueBind("test-queue", "demoExchange", "directexchange_key");

            Console.WriteLine("Creating Queue");

           //Register consumin event
            var consumer = new EventingBasicConsumer(m_model);
            consumer.Received += AmqpMsgPublishReceived;
            var consumerTag = m_model.BasicConsume("spe-queue", true, consumer);


            Console.ReadLine();
        }

        public static IConnection InitiateConnection()
        {
            if (m_connection == null || !m_connection.IsOpen)
            {

                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest"
                };
                m_connection = factory.CreateConnection();
            }

            m_model = m_connection.CreateModel();
            return m_connection;
        }
        public static void TestCallBacmMethod(string receivedMessage)
        {
            Console.WriteLine("Received message - "+receivedMessage);
        }
        private static void AmqpMsgPublishReceived(object sender, BasicDeliverEventArgs ex)
        {
            var arguments = new object[1];
            arguments[0] = Encoding.UTF8.GetString(ex.Body.Span);
            m_callback?.Invoke(arguments[0].ToString());
        }   

    }


}
