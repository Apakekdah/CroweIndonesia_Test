using CI.Interface;
using System;

namespace CI.Model.Models
{
    public abstract class BaseModel : IBaseModel
    {
        public string CreateBy { get; set; }
        public string ModifyBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
