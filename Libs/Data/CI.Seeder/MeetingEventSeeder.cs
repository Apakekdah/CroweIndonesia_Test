using CI.Data.Business.Repositories;
using CI.Data.Entity;
using Hero.Core.Commons;
using Hero.Core.Interfaces;
using Hero.IoC;
using System;
using System.Threading.Tasks;

namespace CI.Seeder
{
    class MeetingEventSeeder : Disposable, IAutoStartService
    {
        private readonly IDisposableIoC life;

        public MeetingEventSeeder(IDisposableIoC life)
        {
            this.life = life.New;
        }

        public int Priority => 0;

        protected override void DisposeCore()
        {
            life.Dispose();
            base.DisposeCore();
        }

        public async Task DoWork()
        {
            var createBy = GetType().Name;

            MeetingEvent meetingEvent;
            var bll = life.GetInstance<MeetingEvents>();
            string meetName;
            DateTime date = DateTime.Now,
                date2 = date.AddHours(1);

            for (var n = 1; n < 30; n++)
            {
                meetName = n.ToString().PadLeft(4, '0');
                meetingEvent = new MeetingEvent
                {
                    Name = $"ME-{meetName}",
                    Description = $"Meeting event - {meetName}",
                    StartDate = date,
                    EndDate = date2,

                    IsActive = true,
                    CreateBy = createBy,
                    ModifyBy = createBy,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                };

                if (!await bll.Exists(meetName).ConfigureAwait(false))
                {
                    await bll.Add(meetingEvent).ConfigureAwait(false);
                }
            }

            await bll.Commit().ConfigureAwait(false);
        }
    }
}
