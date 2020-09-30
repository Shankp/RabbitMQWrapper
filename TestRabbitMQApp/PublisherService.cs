using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client.Framing;

namespace TestRabbitMQApp
{
    public class PublisherService
    {
        private static IModel m_model;
        private static IConnection m_connection;
        private static RabbitMQServerInfo rabbitInfo;
        private static int m_deliveryTagReceived;

        public static int Publisher(string queueName, string message)
        {
            rabbitInfo = ConfigManager.ReadConfigValue();


            InitiateConnection();

            m_model.BasicAcks += ReceiveDeliveryTagFromRabbitMq;

            m_model.ConfirmSelect();

            var props = m_model.CreateBasicProperties();
            props.Persistent = true;
           
            Console.WriteLine("Publishing message to the queue - "+ queueName);

            m_model.BasicPublish(string.Empty, queueName,true, props, Encoding.Default.GetBytes(message));
            return m_deliveryTagReceived; 

        }

        private static void ReceiveDeliveryTagFromRabbitMq(object sender, BasicAckEventArgs e)
        {
            m_deliveryTagReceived = (int)e.DeliveryTag;
        }

        private static IConnection InitiateConnection()
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
    }

}
