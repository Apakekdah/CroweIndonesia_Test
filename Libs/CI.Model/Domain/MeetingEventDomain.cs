using CI.Interface;
using System;

namespace CI.Model.Domain
{
    public class MeetingEventDomain : IBaseModelCreate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
    }
}