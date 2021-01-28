namespace Host.IJobs.Model
{
    public class LogModel
    {
        /// <summary>
        /// 开始执行时间
        /// </summary>
        public string BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 耗时（秒）
        /// </summary>
        public string ExecuteTime { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }      
        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 异常消息
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}
