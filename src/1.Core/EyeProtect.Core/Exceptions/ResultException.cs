using EyeProject.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Core.Exceptions
{
    /// <summary>
    /// 返回指定的 Result 异常信息
    /// </summary>
    public class ResultException : Exception
    {
        /// <inheritdoc cref="ResultException"/>
        public ResultException(ResultCode code, string message = null)
        {
            Result = new Result(code, message);
        }

        /// <inheritdoc cref="ResultException"/>
        public ResultException(IResult result) : base(result.Message)
        {
            Result = result;
        }

        public IResult Result { get; }
    }
}
