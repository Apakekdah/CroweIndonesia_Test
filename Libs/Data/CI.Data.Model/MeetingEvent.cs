using CI.Interface;
using Ride.Attributes.DB;
using System;

namespace CI.Data.Entity
{
    [TableName("td_MeetingEvent")]
    public class MeetingEvent : IBaseModel
    {
        [FieldKey]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public string CreateBy { get; set; }
        public string ModifyBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}