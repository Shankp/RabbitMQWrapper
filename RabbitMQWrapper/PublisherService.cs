using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client.Framing;
using System.IO;
using System.Threading;
using RabbitMQ.Client.Exceptions;

namespace RabbitMQWrapper
{
    public class PublisherService
    {
        private static IModel m_model;
        private static IConnection m_connection;
        private static RabbitMQServerInfo rabbitInfo;
        private static int m_deliveryTagReceived;
        private static int reconntingAttempts;
        public static int? QueueExpiry { get; set; }

        public PublisherService()
        {
            InitiateConnection();
        }

        public static int Publisher(string queueName, string message)
        {
            rabbitInfo = ConfigManager.ReadConfigValue();
            reconntingAttempts = rabbitInfo.RabbitMQReconnectingAttempts;

            try
            {
                Connect();

                if (m_model == null || m_model.IsClosed)
                {
                    InitiateConnection();
                }

                var queueProperty = new Dictionary<string, object> { { "x-expires", QueueExpiry } };
                m_model.QueueDeclare(queueName, true, false, false, QueueExpiry.HasValue ? queueProperty : null);
                m_model.BasicAcks += ReceiveDeliveryTagFromRabbitMq;
                m_model.ConfirmSelect();
                var props = m_model.CreateBasicProperties();
                props.Persistent = true;

                Console.WriteLine("Publishing message to the queue - " + queueName);

                m_model.BasicPublish(string.Empty, queueName, true, props, Encoding.Default.GetBytes(message));
                m_model.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
                return m_deliveryTagReceived;
            }
            catch (Exception)
            {
                throw;
            }


        }

        private static void ReceiveDeliveryTagFromRabbitMq(object sender, BasicAckEventArgs e)
        {
            m_deliveryTagReceived = (int)e.DeliveryTag;
        }

        private static IConnection InitiateConnection()
        {
            try
            {
                if (m_connection == null || !m_connection.IsOpen)
                {

                    var factory = new ConnectionFactory
                    {
                        HostName = rabbitInfo.RabbitMqHostNameAMQP,
                        UserName = rabbitInfo.RabbitMqUsernameAMQP,
                        Password = rabbitInfo.RabbitMqPwdAMQP,
                        NetworkRecoveryInterval = TimeSpan.FromSeconds(2)                        
                    };
                    m_connection = factory.CreateConnection();
                }
                //m_connection.ConnectionShutdown += Connection_ConnectionShutdown;
                m_model = m_connection.CreateModel();
                return m_connection;
            }
            catch (BrokerUnreachableException ex)
            {
                //Reconnect();
                //return m_connection;   
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //private static void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        //{
        //    Console.WriteLine("Connection broke!");

        //    Connect();
        //}

        private static void Connect()
        {
            if (m_model != null && m_model.IsOpen)
            {
                return;
            }

            Cleanup();
            var mres = new ManualResetEventSlim(false);
            while (!mres.Wait(3000) && reconntingAttempts > 0)
            {
                try
                {
                    reconntingAttempts--;
                    InitiateConnection();

                    Console.WriteLine("Connected!");
                    mres.Set();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connect failed!");
                }
            }

            if (m_connection == null || !m_connection.IsOpen)
            {
                Cleanup();
                Console.WriteLine("Exceeds retry attempts.");
                //throw new Exception("Exceeds retry attempts.");
            }

        }

        static void Cleanup()
        {
            try
            {
                if (m_model != null && m_model.IsOpen)
                {
                    m_model.Close();
                    m_model = null;
                }

                if (m_connection != null && m_connection.IsOpen)
                {
                    m_connection.Close();
                    m_connection = null;
                }
            }
            catch (IOException ex)
            {
                // Close() may throw an IOException if connection
                // dies - but that's ok (handled by reconnect)
            }
        }
    }

}
