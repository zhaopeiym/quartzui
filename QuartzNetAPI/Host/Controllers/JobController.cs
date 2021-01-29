using Host.Common;
using Host.Entity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using System.Collections.Generic;
using System.Threading.Tasks;
using Talk.Extensions;

namespace Host.Controllers
{
    /// <summary>
    /// 任务调度
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [EnableCors("AllowSameDomain")] //允许跨域 
    public class JobController : Controller
    {
        private SchedulerCenter scheduler;

        /// <summary>
        /// 任务调度对象
        /// </summary>
        /// <param name="schedulerCenter"></param>
        public JobController(SchedulerCenter schedulerCenter)
        {
            scheduler = schedulerCenter;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResult> AddJob([FromBody] ScheduleEntity entity)
        {
            if (ConfigurationManager.GetTryConfig("EnvironmentalRestrictions", "false") == "true")
            {
                if (entity.TriggerType == TriggerTypeEnum.Simple &&
                    entity.IntervalSecond.HasValue &&
                    entity.IntervalSecond <= 10)
                {
                    return new BaseResult()
                    {
                        Code = 403,
                        Msg = "当前环境不允许低于10秒内循环执行任务！"
                    };
                }
                else if (entity.TriggerType == TriggerTypeEnum.Cron &&
                         entity.Cron == "* * * * * ?")
                {
                    return new BaseResult()
                    {
                        Code = 403,
                        Msg = "当前环境不允许过频繁执行任务！"
                    };
                }

            }
            return await scheduler.AddScheduleJobAsync(entity);
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResult> StopJob([FromBody] JobKey job)
        {
            return await scheduler.StopOrDelScheduleJobAsync(job.Group, job.Name);
        }

        /// <summary>
        /// 删除任务
        /// </summary> 
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResult> RemoveJob([FromBody] JobKey job)
        {
            return await scheduler.StopOrDelScheduleJobAsync(job.Group, job.Name, true);
        }

        /// <summary>
        /// 恢复运行暂停的任务
        /// </summary> 
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResult> ResumeJob([FromBody] JobKey job)
        {
            return await scheduler.ResumeJobAsync(job.Group, job.Name);
        }

        /// <summary>
        /// 查询任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ScheduleEntity> QueryJob([FromBody] JobKey job)
        {
            return await scheduler.QueryJobAsync(job.Group, job.Name);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResult> ModifyJob([FromBody] ModifyJobInput entity)
        {
            if (ConfigurationManager.GetTryConfig("EnvironmentalRestrictions", "false") == "true")
            {

                if (entity.NewScheduleEntity.TriggerType == TriggerTypeEnum.Simple &&
                    entity.NewScheduleEntity.IntervalSecond.HasValue &&
                    entity.NewScheduleEntity.IntervalSecond <= 10)
                {
                    return new BaseResult()
                    {
                        Code = 403,
                        Msg = "当前环境不允许低于10秒内循环执行任务！"
                    };
                }
                else if (entity.NewScheduleEntity.TriggerType == TriggerTypeEnum.Cron &&
                    entity.NewScheduleEntity.Cron == "* * * * * ?")
                {
                    return new BaseResult()
                    {
                        Code = 403,
                        Msg = "当前环境不允许过频繁执行任务！"
                    };
                }
            }

            var jobKey = new JobKey(entity.OldScheduleEntity.JobName, entity.OldScheduleEntity.JobGroup);
            var runNumber = await scheduler.GetRunNumberAsync(jobKey);
            await scheduler.StopOrDelScheduleJobAsync(entity.OldScheduleEntity.JobGroup, entity.OldScheduleEntity.JobName, true);
            await scheduler.AddScheduleJobAsync(entity.NewScheduleEntity, runNumber);
            return new BaseResult() { Msg = "修改计划任务成功！" };
        }

        /// <summary>
        /// 立即执行
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> TriggerJob([FromBody] JobKey job)
        {
            await scheduler.TriggerJobAsync(job);
            return true;
        }

        /// <summary>
        /// 获取job日志
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<string>> GetJobLogs([FromBody] JobKey jobKey)
        {
            var logs = await scheduler.GetJobLogsAsync(jobKey);
            logs?.Reverse();
            return logs;
        }

        /// <summary>
        /// 启动调度
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<bool> StartSchedule()
        {
            return await scheduler.StartScheduleAsync();
        }

        /// <summary>
        /// 停止调度
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<bool> StopSchedule()
        {
            return await scheduler.StopScheduleAsync();
        }

        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<JobInfoEntity>> GetAllJob()
        {
            return await scheduler.GetAllJobAsync();
        }

        /// <summary>
        /// 获取所有Job信息（简要信息 - 刷新数据的时候使用）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<JobBriefInfoEntity>> GetAllJobBriefInfo()
        {
            return await scheduler.GetAllJobBriefInfoAsync();
        }

        /// <summary>
        /// 移除异常信息
        /// </summary>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> RemoveErrLog([FromBody] JobKey jobKey)
        {
            return await scheduler.RemoveErrLog(jobKey.Group, jobKey.Name);
        }
    }
}
