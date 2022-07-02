using CI.Data.Entity;
using Hero.Business;
using Hero.Core.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public Task<IEnumerable<MeetingEvent>> GetAllLimit()
        {
            return Task.Run(() => GetQuery().Take(1000).ToArray().AsEnumerable());
        }

        public async Task<int> AddSeeds(IEnumerable<MeetingEvent> meetingEvents)
        {
            int numberOfRows = 0;
            var partition = meetingEvents.Split(10);
            IEnumerable<string> lstNames;
            IEnumerable<MeetingEvent> meetingEventsSaveds,
                meetingEventsNotSaved;
            foreach (var part in partition)
            {
                lstNames = part.Select(c => c.Name).ToArray();
                meetingEventsSaveds = await GetMany(c => lstNames.Contains(c.Name)).ConfigureAwait(false);
                if (meetingEventsSaveds.Any())
                {
                    meetingEventsNotSaved = part.Except(meetingEventsSaveds, new MeetingEventNameComparer()).ToArray();
                    if (!meetingEventsNotSaved.Any())
                        continue;
                }
                else
                {
                    meetingEventsNotSaved = part;
                }
                await AddMany(meetingEventsNotSaved).ConfigureAwait(false);
                numberOfRows += meetingEventsNotSaved.Count();
            }
            partition.Clear();
            return numberOfRows;
        }
    }

    class MeetingEventNameComparer : IEqualityComparer<MeetingEvent>
    {
        public bool Equals([AllowNull] MeetingEvent x, [AllowNull] MeetingEvent y)
        {
            return x?.Name.Equals(y?.Name) == true;
        }

        public int GetHashCode([DisallowNull] MeetingEvent obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}