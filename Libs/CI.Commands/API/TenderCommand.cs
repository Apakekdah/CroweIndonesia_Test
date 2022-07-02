using Ride.Attributes.Validator;
using System;
using System.ComponentModel.DataAnnotations;

namespace CI.Commands.API
{
    public class TenderCommandCU : BaseCommand
    {
        public string ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ReferenceNumber { get; set; }
        [Required]
        [DateValidator(-1)]
        public DateTime ReleaseDate { get; set; }
        [Required]
        [DateValidator(0)]
        public DateTime ClosingDate { get; set; }
        [Required]
        public string Details { get; set; }
    }

    public class TenderCommandD : BaseCommand
    {
        public string ID { get; set; }
    }

    public class TenderCommandRA : BaseCommand
    {
        public string ID { get; set; }
    }
}