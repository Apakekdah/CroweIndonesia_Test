using CI.Interface;
using Hero;
using System;

namespace CI
{
    public class CIJsonResult
    {
        public bool Success { get; set; }
        public object Rows { get; set; }
        public int RowCount { get; set; }
        public Exception Error { get; set; }

        public static CIJsonResult ParseFromCommandResult<T>(ICommandResultWithCount<T> result)
        {
            result.ThrowIfNull("result");

            return new CIJsonResult()
            {
                Error = result.Ex,
                RowCount = result.RowCount,
                Rows = result.Result,
                Success = result.Success
            };
        }
    }
}