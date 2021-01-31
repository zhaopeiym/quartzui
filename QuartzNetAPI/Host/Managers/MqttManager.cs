using Host.Common;
using Host.Common.Enums;
using Host.Entity;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Host.Managers
{
    /// <summary>
    /// Mqtt - 单例
    /// </summary>
    public class MqttManager
    {
        public static readonly MqttManager Instance;
        static MqttManager()
        {
            Instance = new MqttManager();
        }

        public IManagedMqttClient MqttClient { get; private set; }

        /// <summary>
        /// 重启启动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task RestartAsync()
        {
            try
            {
                await StopAsync();

                var model = await FileConfig.GetMqttSetAsync();
                MqttClient = new MqttFactory().CreateManagedMqttClient();
                var mqttClientOptions = new MqttClientOptionsBuilder()
                             .WithKeepAlivePeriod(TimeSpan.FromSeconds(29))
                             .WithClientId(model.ClientId)
                             .WithWebSocketServer($"{model.Host}:{model.Port}/mqtt")
                             .WithCredentials(model.UserName, model.Password);

                if (model.ConnectionMethod == ConnectionMethod.WSS)
                    mqttClientOptions = mqttClientOptions.WithTls();

                var options = new ManagedMqttClientOptionsBuilder()
                       .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                       .WithClientOptions(mqttClientOptions.Build())
                       .Build();

                await MqttClient.StartAsync(options);
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"MQTT启动异常,{ex.Message}");
            }
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topic"></param>
        /// <param name="payloadData"></param>
        /// <param name="retain"></param>
        /// <param name="serviceLevel"></param>
        /// <returns></returns>
        public async Task<MqttClientPublishResult> PublishAsync<T>(string topic, T payloadData, bool retain = false, MqttQualityOfServiceLevel serviceLevel = MqttQualityOfServiceLevel.AtMostOnce) //where T : class, new()
        {
            var payload = JsonConvert.SerializeObject(payloadData, Formatting.None, AppSetting.SerializerSettings);
            return await PublishAsync(topic, payload, retain);
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="payload"></param>
        /// <param name="retain"></param>
        /// <param name="serviceLevel"></param>
        /// <returns></returns>
        public async Task<MqttClientPublishResult> PublishAsync(string topic, string payload, bool retain = false, MqttQualityOfServiceLevel serviceLevel = MqttQualityOfServiceLevel.AtMostOnce)
        {
            return await MqttClient.PublishAsync(topic, payload, serviceLevel, retain);
        }

        /// <summary>
        /// Stop
        /// </summary>
        /// <returns></returns>
        private async Task StopAsync()
        {
            if (MqttClient?.IsStarted ?? false)
                await MqttClient?.StopAsync();
        }
    }
}
