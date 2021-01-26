using System;

namespace Host.Attributes
{
    /// <summary>
    /// 标记了此特性的方法，不需要进行登录和授权认证
    /// </summary>
    public class NoLoginAttribute : Attribute
    {
    }
}
