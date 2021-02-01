namespace Host.IJobs.Model
{
    public class LogMqttModel : LogModel
    {
        public string Topic { get; set; }
        public string Payload { get; set; }
    }
}
