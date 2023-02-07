using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EyeProject.Core.Dto
{
    /// <summary>
    /// 扩展 <see cref="IResult"/> 类
    /// </summary>
    public static class ResultExtension
    {
        /// <summary>
        /// 是否为成功状态
        /// </summary>
        public static bool IsOk(this IResult result)
        {
            return result?.Code == ResultCode.Ok;
        }

        /// <summary>
        /// 返回指定 Code
        /// </summary>
        public static TResult ByCode<TResult>(this TResult result, ResultCode code, string message = null)
            where TResult : IResult
        {
            result.Code = code;
            if (message != null)
                result.Message = message;
            return result;
        }

        /// <summary>
        /// 返回异常信息
        /// </summary>
        public static TResult ByError<TResult>(this TResult result, string message)
            where TResult : IResult
        {
            result.Code = ResultCode.Fail;
            if (message != null)
                result.Message = message;
            return result;
        }

        /// <summary>
        /// 返回异常信息
        /// </summary>
        public static TResult ByError<TResult>(this TResult result, string message, ResultCode code)
            where TResult : IResult
        {
            result.Code = code;
            if (message != null)
                result.Message = message;
            return result;
        }

        /// <summary>
        /// 返回指定 SubCode
        /// </summary>
        public static TResult BySubCode<TResult, TSubCode>(this TResult result, TSubCode subCode,
            string subMessage = null, ResultCode code = default, string message = null)
            where TResult : ISubCodeResult<TSubCode>
            where TSubCode : struct, Enum
        {
            result.SubCode = subCode;
            result.SubMessage = subMessage;
            return result.ByCode(code, message);
        }

        /// <summary>
        /// 返回指定 数据
        /// </summary>
        public static TResult BySubCode<TResult, TSubCode, TData>(this TResult result, TData data,
            TSubCode subCode = default, string subMessage = null, ResultCode code = default, string message = null)
            where TResult : ISubCodeResult<TSubCode, TData>
            where TSubCode : struct, Enum
        {
            return result.ByData(data).BySubCode(subCode, subMessage, code, message);
        }

        /// <summary>
        /// 返回指定 subCode 的 <see cref="SubCodeResult{TSubCode}"/>
        /// </summary>
        public static SubCodeResult<TSubCode> ToSubCode<TSubCode>(this IResult result,
            TSubCode subCode,
            string subMessage = null)
            where TSubCode : struct, Enum
        {
            return new SubCodeResult<TSubCode>().ByResult(result).BySubCode(subCode, subMessage);
        }

        /// <summary>
        /// 返回指定 subCode 的 <see cref="SubCodeResult{TSubCode, TData}"/>
        /// </summary>
        public static SubCodeResult<TSubCode, TData> ToSubCode<TSubCode, TData>(this IResult result,
            TSubCode subCode,
            string subMessage = null)
            where TSubCode : struct, Enum
        {
            return new SubCodeResult<TSubCode, TData>().ByResult(result).BySubCode(subCode, subMessage);
        }

        /// <summary>
        /// 返回指定 subCode 的 <see cref="SubCodeResult{TSubCode, TData}"/>
        /// </summary>
        public static SubCodeResult<TSubCode, TData> ToSubCode<TSubCode, TData>(this IResult result,
            TSubCode subCode, TData data, string subMessage = null)
            where TSubCode : struct, Enum
        {
            return new SubCodeResult<TSubCode, TData>().ByResult(result).BySubCode(data, subCode, subMessage);
        }

        /// <summary>
        /// 返回指定 subCode 的 <see cref="SubCodeResult{TSubCode, TData}"/>
        /// </summary>
        public static SubCodeResult<TSubCode, TData> ToSubCode<TSubCode, TData>(this IResult<TData> result,
            TSubCode subCode, string subMessage = null)
            where TSubCode : struct, Enum
        {
            return new SubCodeResult<TSubCode, TData>().ByResultData(result).BySubCode(subCode, subMessage);
        }

        /// <summary>
        /// 返回新结果
        /// </summary>
        public static TResult ByResult<TResult>(this TResult result, IResult otherResult)
            where TResult : IResult
        {
            result.Code = otherResult.Code;
            result.Message = otherResult.Message;
            return result;
        }

        /// <summary>
        /// 返回新结果
        /// </summary>
        public static TResult ByResultData<TResult, TData>(this TResult result, IResult<TData> otherResult)
            where TResult : IResult<TData>
        {
            result.Code = otherResult.Code;
            result.Message = otherResult.Message;
            result.Data = otherResult.Data;
            return result;
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        public static TResult ByData<TResult, TData>(this TResult result, TData data)
            where TResult : IResult<TData>
        {
            result.Code = ResultCode.Ok;
            result.Message = null;
            result.Data = data;
            return result;
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        public static TResult ByOk<TResult>(this TResult result, string message = null)
            where TResult : IResult
        {
            result.Code = ResultCode.Ok;
            result.Message = message;
            return result;
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        public static TResult ByOk<TResult, TData>(this TResult result, TData data)
            where TResult : IResult<TData>
        {
            result.Code = ResultCode.Ok;
            result.Message = null;
            result.Data = data;
            return result;
        }


        /// <summary>
        /// 返回结果数据
        /// </summary>
        public static async Task<Result> ToResultAsync(this Task task)
        {
            await task;
            return Result.Ok();
        }

        /// <summary>
        /// 返回结果数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="noRecode">如果 <see cref="Result{T}.Data"/> 为 Null，则状态码为 <see cref="ResultCode.NoRecord"/></param>
        public static async Task<Result<T>> ToResultAsync<T>(this Task<T> data, bool noRecode = false)
        {
            var rs = Result.FromData(await data);
            if (noRecode && rs.Data == null)
                rs.Code = ResultCode.NoRecord;

            return rs;
        }

        /// <summary>
        /// 返回列表数据
        /// </summary>
        public static async Task<ListResult<TElement>> ToListResultAsync<TCollection, TElement>(
            this Task<TCollection> source)
            where TCollection : IEnumerable<TElement>
        {
            var result = await source;
            return new ListResult<TElement>(result.ToList());
        }
    }
}
