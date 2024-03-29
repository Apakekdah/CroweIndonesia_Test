﻿using CI.Data.Business.Repositories;
using CI.Data.Entity;
using Hero.Core.Commons;
using Hero.Core.Interfaces;
using Hero.IoC;
using System;
using System.Collections.Generic;
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

            var bll = life.GetInstance<MeetingEvents>();
            string meetName;
            DateTime now = DateTime.Now,
                date = now.Date,
                date2 = date;

            ICollection<MeetingEvent> colMeets = new HashSet<MeetingEvent>();

            for (var n = 1; n < 30; n++)
            {
                meetName = n.ToString().PadLeft(4, '0');

                colMeets.Add(new MeetingEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"ME-{meetName}",
                    Description = $"Meeting event - {meetName}",
                    StartDate = date,
                    EndDate = date2,

                    IsActive = true,
                    CreateBy = createBy,
                    ModifyBy = createBy,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                });

                date.AddDays(n);
                date2.AddDays(n + 1);
            }

            if (await bll.AddSeeds(colMeets) > 0)
            {

                await bll.Commit().ConfigureAwait(false);
            }

            colMeets.Clear();
        }
    }
}
