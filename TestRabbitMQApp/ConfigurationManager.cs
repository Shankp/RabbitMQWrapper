using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace TestRabbitMQApp
{
    public class ConfigManager
    {
        public static RabbitMQServerInfo ReadConfigValue()
        {
            return new RabbitMQServerInfo
            {
                RabbitMqHostNameAMQP = ConfigurationManager.AppSettings["RabbitMqHostName"],
                RabbitMqUsernameAMQP = ConfigurationManager.AppSettings["RabbitMqUsernameAMQP"],
                RabbitMqPwdAMQP = ConfigurationManager.AppSettings["RabbitMqPwdAMQP"]
            };
        }
    }
}
