using CI.Interface;
using CI.Model.Models;

namespace CI.Model.Domain
{
    public class MeetingEventDomain : MeetingEvent, IPaginator
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}