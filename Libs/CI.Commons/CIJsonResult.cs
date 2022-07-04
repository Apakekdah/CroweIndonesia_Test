using CI.Interface;
using Hero;
using Hero.Core.Interfaces;
using System;

namespace CI
{
    public class CIResponse
    {
        public bool Success { get; set; }

        public CIJsonResult Result { get; set; }

        public static CIResponse ParseFromCommandResult<T>(ICommandResult<T> result)
        {
            result.ThrowIfNull("result");

            ICommandResultWithCount<T> newResult = null;
            if (typeof(ICommandResultWithCount<T>).IsAssignableFrom(result.GetType()))
            {
                newResult = (ICommandResultWithCount<T>)result;
            }

            var dataResult = new CIJsonResult
            {
                Error = result.Ex,
                Rows = result.Result,

                RowCount = newResult?.RowCount ?? 0,
                Page = newResult?.Page ?? 0,
                PageSize = newResult?.PageSize ?? 0,
            };
            return new CIResponse()
            {
                Success = result.Success,
                Result = dataResult
            };
        }
    }

    public class CIJsonResult
    {
        public object Rows { get; set; }
        public int RowCount { get; set; }
        public Exception Error { get; set; }
        public string Query { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}