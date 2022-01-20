using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQWrapper
{
    public class SubscriberService
    {
        private static IModel m_model;
        private static IConnection m_connection;
        private static Action<string> m_callback;
        private static RabbitMQServerInfo rabbitInfo;
        

        public static string Subscriber(string queueName)
        {
            rabbitInfo = ConfigManager.ReadConfigValue();

            if (m_model == null || m_model.IsClosed)
                InitiateConnection();

            m_callback = TestCallBacmMethod;

            //Declaring exchange
            m_model.ExchangeDeclare("demoExchange", ExchangeType.Direct);

            //Declare queue
            m_model.QueueDeclare(queueName, true, false, false, null);

            //Bind queue to the exchange
            m_model.QueueBind(queueName, "demoExchange", "directexchange_key");

            Console.WriteLine("Subscribing Queue : "+ queueName + Environment.NewLine);

            //Register consumin event
            var consumer = new EventingBasicConsumer(m_model);
            consumer.Received += AmqpMsgPublishReceived;
            return m_model.BasicConsume(queueName, true, consumer);            
        }

        public static IConnection InitiateConnection()
        {
            if (m_connection == null || !m_connection.IsOpen)
            {

                var factory = new ConnectionFactory
                {
                    HostName = rabbitInfo.RabbitMqHostNameAMQP,
                    UserName = rabbitInfo.RabbitMqUsernameAMQP,
                    Password = rabbitInfo.RabbitMqPwdAMQP
                };
                m_connection = factory.CreateConnection();
            }

            m_model = m_connection.CreateModel();
            return m_connection;
        }

      
        public static void TestCallBacmMethod(string receivedMessage)
        {
            Console.WriteLine("Received message - " + receivedMessage);
        }
        private static void AmqpMsgPublishReceived(object sender, BasicDeliverEventArgs ex)
        {
            var arguments = new object[1];
            arguments[0] = Encoding.UTF8.GetString(ex.Body.Span);
            m_callback?.Invoke(arguments[0].ToString());
        }       
    }
}
