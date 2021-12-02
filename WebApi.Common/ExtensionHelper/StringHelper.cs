using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Common.ExtensionHelper
{
    public static class StringHelper
    {
        /// <summary>
        /// 字符串是否合法(非空）
        /// </summary>
        /// <param name="self">字符串</param>
        /// <param name="rule">验证规则</param>
        /// <returns></returns>
        public static bool IsLegal(this string self, Func<string, bool> rule = null)
        {
            if (string.IsNullOrEmpty(self))
            {
                return false;
            }
            if (rule != null)
            {
                return rule.Invoke(self);
            }
            return true;
        }
    }
}
