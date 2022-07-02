using CI.Attributes;
using CI.Interface;
using Ride.Attributes.Validator;
using System;
using System.ComponentModel.DataAnnotations;

namespace CI.Commands.API
{
    public class MeetingEventCommandCU : BaseCommand, IBaseModelCreate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        [DateValidator(0)]
        [DateRange("EndDate", false)]
        public DateTime StartDate { get; set; }
        [Required]
        [DateValidator(0)]
        [DateRange("StartDate", false, false)]
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class MeetingEventCommandD : BaseCommand, IBaseModelModify
    {
        [Required]
        public string Id { get; set; }

        public string ModifyBy { get; set; }
        public DateTime ModifyDate { get; set; }
    }

    public class MeetingEventCommandRA : BaseCommand, IPaginator
    {
        public string Id { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}