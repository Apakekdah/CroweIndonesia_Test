using Hero.Core.Interfaces;

namespace CI.Interface
{
    public interface ICommandResultWithCount<T> : ICommandResult<T>
    {
        public int RowCount { get; set; }
    }
}