using Host.Common;
using Host.Entity;
using Newtonsoft.Json;
using Quartz;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Host.Tests
{
    public class HttpHelperTests
    {
        private string httpBase = "http://localhost:53118";

        [Fact]
        public async Task TestPostAsync()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Authorization", "QPWBVpMZP+DHWmn502ebwlr4FI21zZrVuk8nHuFrTPQ=");         

            var msg2 = await HttpHelper.Instance.PostAsync("http://localhost:60156/api/InspectionOrder/GenerateInspectionOrderJob", "", dic);
            var msg = await HttpHelper.Instance.PostAsync("http://localhost:60156/api/MaintenanceOrder/GenerateMaintenanceOrderJob", "", dic);


            var entity = new ScheduleEntity();
            entity.TriggerType = TriggerTypeEnum.Simple;
            entity.JobName = "JobNameBenny";
            entity.JobGroup = "JobGroupBenny";
            entity.IntervalSecond = 12;
            var addUrl = httpBase + "/api/Job/AddJob";
            //添加测试数据
            var resultStr = await HttpHelper.Instance.PostAsync(addUrl, JsonConvert.SerializeObject(entity));
            var addResult = JsonConvert.DeserializeObject<BaseResult>(resultStr.Content.ReadAsStringAsync().Result);

            //验证
            Assert.True(addResult.Code == 200);

            //删除测试数据
            var key = new JobKey(entity.JobName, entity.JobGroup);
            var delUrl = httpBase + "/api/Job/RemoveJob";
            var delResultStr = await HttpHelper.Instance.PostAsync(delUrl, JsonConvert.SerializeObject(key));
            var delResult = JsonConvert.DeserializeObject<BaseResult>(delResultStr.Content.ReadAsStringAsync().Result);
            Assert.True(delResult.Code == 200);
        }

        [Fact]
        public async Task TestGetAsync()
        {
            var url = httpBase + "/api/Job/GetAllJob";
            var obj = await HttpHelper.Instance.GetAsync(url);
            var result = JsonConvert.DeserializeObject<List<JobBriefInfoEntity>>(obj.Content.ReadAsStringAsync().Result);
            Assert.True(result != null);
        }

        [Fact]
        public async Task TestPutAsync()
        {
            var url = "http://localhost:50090/api/Values/123";
            var obj = await HttpHelper.Instance.PutAsync(url, "{Version:123}");
        }

        [Fact]
        public async Task TestDeleteAsync()
        {
            var url = "http://localhost:50090/api/Values/123";
            var obj = await HttpHelper.Instance.DeleteAsync(url);
        }
    }
}
