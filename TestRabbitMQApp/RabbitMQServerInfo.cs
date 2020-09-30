using System;
using System.Collections.Generic;
using System.Text;

namespace TestRabbitMQApp
{
    public class RabbitMQServerInfo
    {
        public string RabbitMqHostNameAMQP { get; set; }
        public string RabbitMqUsernameAMQP { get; set; }
        public string RabbitMqPwdAMQP { get; set; }
        public string RabbitMqVirtualHostAMQP { get; set; }
        public int RabbitMqPortAMQP { get; set; }
        public string RabbitMqHostNameMQTT { get; set; }
        public string RabbitMqUsernameMQTT { get; set; }
        public string RabbitMqPwdMQTT { get; set; }
        public string RabbitMqVirtualHostMQTT { get; set; }
        public int RabbitMqPortMQTT { get; set; }
    }
}
