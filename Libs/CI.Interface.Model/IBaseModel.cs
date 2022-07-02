using System;

namespace CI.Interface
{
    public interface IBaseModel : IBaseModelCreate, IBaseModelModify
    {
    }

    public interface IBaseModelCreate
    {
        string CreateBy { get; set; }
        DateTime CreateDate { get; set; }
    }

    public interface IBaseModelModify
    {
        string ModifyBy { get; set; }
        DateTime ModifyDate { get; set; }
    }
}