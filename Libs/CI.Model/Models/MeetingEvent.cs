using System;

namespace CI.Model.Models
{
    public class MeetingEvent : BaseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public string StartDateFormatted => StartDate.ToString("yyyy-MM-dd");
        public string EndDateFormatted => EndDate.ToString("yyyy-MM-dd");
        public string CreateDateFormatted => CreateDate.ToString("yyyy-MM-dd");
        public string ModifyDateFormatted => ModifyDate.ToString("yyyy-MM-dd");
        public string MeetingStatusFormatted => (IsActive ? "Active" : "Not Active");
    }
}