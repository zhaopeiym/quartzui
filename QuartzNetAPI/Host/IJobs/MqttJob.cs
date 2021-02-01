using Host.Common;
using Host.IJobs.Model;
using Host.Managers;
using MQTTnet.Client.Publishing;
using Newtonsoft.Json;
using Quartz;
using System.Threading.Tasks;

namespace Host.IJobs
{
    public class MqttJob : JobBase<LogMqttModel>, IJob
    {
        private MqttManager mqttManager;
        public MqttJob() : base(new LogMqttModel())
        {
            mqttManager = MqttManager.Instance;
        }

        public override async Task NextExecute(IJobExecutionContext context)
        {
            var topic = context.JobDetail.JobDataMap.GetString(Constant.Topic);
            var payload = context.JobDetail.JobDataMap.GetString(Constant.Payload);
            LogInfo.Topic = topic;
            LogInfo.Payload = payload;

            var mqttSet = await FileConfig.GetMqttSetAsync();
            if (string.IsNullOrWhiteSpace(mqttSet.Host) || string.IsNullOrWhiteSpace(mqttSet.Port))
                LogInfo.ErrorMsg = $"<span class='error'>请先在 [/seting] 页面配置MQTT设置。</span>";
            else if (!mqttManager.MqttClient.IsConnected)
                LogInfo.ErrorMsg = $"<span class='error'>Mqtt服务连接失败</span>";
            else if (!mqttManager.MqttClient.IsStarted)
                LogInfo.ErrorMsg = $"<span class='error'>Mqtt服务启动失败</span>";
            else
            {
                var detectionrResult = await mqttManager.PublishAsync(topic, payload);
                if (detectionrResult.ReasonCode != MqttClientPublishReasonCode.Success)
                    LogInfo.ErrorMsg = $"<span class='error'>topic:{topic} reason:{detectionrResult.ReasonString} {detectionrResult.ReasonCode}</span>";
            }

            if (!string.IsNullOrWhiteSpace(LogInfo.ErrorMsg))
                context.JobDetail.JobDataMap[Constant.EXCEPTION] = $"<div class='err-time'>{LogInfo.BeginTime}</div>{JsonConvert.SerializeObject(LogInfo)}";
            else
                LogInfo.Result = "发送成功！";
        }
    }
}
