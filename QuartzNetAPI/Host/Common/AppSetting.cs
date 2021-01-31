using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Host.Common
{
    public class AppSetting
    {
        /// <summary>
        /// 小驼峰命名
        /// </summary>
        public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }
}
