using Host.Common.Enums;

namespace Host.Entity
{
    public class MqttOptionsEntity
    {
        public string ClientId { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public ConnectionMethod ConnectionMethod { get; set; }
    }
}
