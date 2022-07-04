using CI.Interface;
using Hero.Core.Commons;
using System;

namespace CI.Commons
{
    public class CommandResultWithCount<T> : CommandResult<T>, ICommandResultWithCount<T>
    {
        public CommandResultWithCount(bool isSuccess) : base(isSuccess)
        {
        }

        public CommandResultWithCount(Exception ex) : base(ex)
        {
        }

        public CommandResultWithCount(bool isSuccess, string addInfo) : base(isSuccess, addInfo)
        {
        }

        public CommandResultWithCount(bool isSuccess, T result, int totalRow) : base(isSuccess, result)
        {
            RowCount = totalRow;
        }

        public int RowCount { get; set; }
        public string Query { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}