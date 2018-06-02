using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Host.Entity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl;

namespace Host.Controllers
{
    /// <summary>
    /// 任务调度
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [EnableCors("AllowSameDomain")] //允许跨域 
    public class JobController : Controller
    {
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResult> AddJob([FromBody]ScheduleEntity entity)
        {
            return await SchedulerCenter.Instance.AddScheduleJob(entity);
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResult> StopJob([FromBody]JobKey job)
        {
            return await SchedulerCenter.Instance.StopOrDelScheduleJob(job.Group, job.Name);
        }

        /// <summary>
        /// 删除任务
        /// </summary> 
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResult> RemoveJob([FromBody]JobKey job)
        {
            return await SchedulerCenter.Instance.StopOrDelScheduleJob(job.Group, job.Name, true);
        }

        /// <summary>
        /// 恢复运行暂停的任务
        /// </summary> 
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResult> ResumeJob([FromBody]JobKey job)
        {
            return await SchedulerCenter.Instance.ResumeJob(job.Group, job.Name);
        }

        /// <summary>
        /// 查询任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ScheduleEntity> QueryJob([FromBody]JobKey job)
        {
            return await SchedulerCenter.Instance.QueryJob(job.Group, job.Name);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<BaseResult> ModifyJob([FromBody]ScheduleEntity entity)
        {
            await SchedulerCenter.Instance.StopOrDelScheduleJob(entity.JobGroup, entity.JobName, true);
            await SchedulerCenter.Instance.AddScheduleJob(entity);
            return new BaseResult() { Msg = "修改计划任务成功！" };
        }

        /// <summary>
        /// 启动调度
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<bool> StartSchedule()
        {
            return await SchedulerCenter.Instance.StartSchedule();
        }

        /// <summary>
        /// 停止调度
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<bool> StopSchedule()
        {
            return await SchedulerCenter.Instance.StopSchedule();
        }

        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<JobInfoEntity>> GetAllJob()
        {
            return await SchedulerCenter.Instance.GetAllJob();
        }

    }
}
