using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Host.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task TestPostAsync()
        {
            var url = "http://localhost:50090/api/Values";
            var obj = await HttpHelper.Instance.PostAsync(url, "{Version:123}");
        }

        [Fact]
        public async Task TestGetAsync()
        {
            var url = "http://localhost:50090/api/Values/123";
            var obj = await HttpHelper.Instance.GetAsync(url);
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
