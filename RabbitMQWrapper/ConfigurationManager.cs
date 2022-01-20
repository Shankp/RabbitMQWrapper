using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace RabbitMQWrapper
{
    public class ConfigManager
    {
        public static RabbitMQServerInfo ReadConfigValue()
        {
            var retryAttempts = ConfigurationManager.AppSettings["RabbitMQReconnectingAttempts"];
            return new RabbitMQServerInfo
            {
                RabbitMqHostNameAMQP = ConfigurationManager.AppSettings["RabbitMqHostName"],
                RabbitMqUsernameAMQP = ConfigurationManager.AppSettings["RabbitMqUsernameAMQP"],
                RabbitMqPwdAMQP = ConfigurationManager.AppSettings["RabbitMqPwdAMQP"],
                RabbitMQReconnectingAttempts = int.Parse(retryAttempts)
            };
        }
    }
}
