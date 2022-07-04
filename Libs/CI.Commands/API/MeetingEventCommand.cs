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
        [MaxLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        [DateValidator(0)]
        [DateRange("EndDate")]
        public DateTime StartDate { get; set; }
        [Required]
        [DateValidator(0)]
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }


        [Required]
        public string CreateBy { get; set; }

        [Required]
        [DateValidator(0)]
        public DateTime CreateDate { get; set; }
    }

    public class MeetingEventCommandD : BaseCommand, IBaseModelModify
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string ModifyBy { get; set; }
        [Required]
        [DateValidator(0)]
        public DateTime ModifyDate { get; set; }
    }

    public class MeetingEventCommandRA : BaseCommand
    {
        public string Id { get; set; }
    }
}