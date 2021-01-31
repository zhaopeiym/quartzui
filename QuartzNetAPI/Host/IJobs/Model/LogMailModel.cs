namespace Host.IJobs.Model
{
    public class LogMqttlModel : LogModel
    {
        public string Topic { get; set; }
        public string Payload { get; set; }
    }
}
