﻿using System;

namespace CI.Model.Models
{
    public class MeetingEvent : BaseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}