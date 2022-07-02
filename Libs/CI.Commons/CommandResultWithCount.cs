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

        public CommandResultWithCount(bool isSuccess, T result) : base(isSuccess, result)
        {
        }

        public int RowCount { get; set; }

        //public static explicit operator CommandResultWithCount<T>(CommandResult<T> result)
        //{
        //    if (result.GetType().IsAssignableFrom(typeof(ICommandResultWithCount<T>)))
        //    {
        //        return (CommandResultWithCount<T>)result;
        //    }

        //    return null;
        //}
    }
}