using Hero.Core.Interfaces;

namespace CI.Interface
{
    public interface ICommandResultWithCount<T> : ICommandResult<T>
    {
        public int RowCount { get; set; }
        public string Query { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}