namespace Host.IJobs.Model
{
    public class LogRabbitModel : LogModel
    {
        public string RabbitQueue { get; set; }
        public string RabbitBody { get; set; }
    }
}
