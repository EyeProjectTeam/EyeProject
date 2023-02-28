using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Core.Utils
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 获取异常的原始异常信息(<see cref="System.Exception.InnerException"/>)
        /// </summary>
        public static Exception GetOriginalException(this Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            return exception;
        }

        public static string GetAllMessage(this Exception exception, string split = " -> ")
        {
            if (exception == null) return null;

            var error = new StringBuilder();
            while (exception != null)
            {
                error.Append(exception.Message).Append(split);
                exception = exception.InnerException;
            }

            return error.ToString();
        }
    }
}
