using CI.Data.Entity;
using Hero.Business;
using Hero.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CI.Data.Business.Repositories
{
    public class MeetingEvents : BusinessClassBaseAsync<MeetingEvent>
    {
        public MeetingEvents(IRepositoryAsync<MeetingEvent> repository, IUnitOfWorkAsync unitOfWork) : base(repository, unitOfWork)
        {
        }

        public Task<MeetingEvent> GetByMeetingEventID(string id)
        {
            return Get(f => f.Id == id);
        }

        public Task<IEnumerable<MeetingEvent>> GetMeetingEventPaging(int page, int pageSize)
        {
            return Task.Run(() =>
            {
                var skip = ((page < 2 ? 0 : page - 1) * pageSize);
                return GetQuery().Take(pageSize).Skip(skip).ToArray().AsEnumerable();
            });
        }
    }
}