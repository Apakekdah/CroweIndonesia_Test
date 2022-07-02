using Hero;
using Hero.Core.Commons;
using Hero.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace CI.Commands
{
    public enum CommandProcessor : int
    {
        GetAll = 0,
        Add,
        Edit,
        Delete,
        GetOne
    }

    public abstract class BaseCommand : Disposable, ICommand, ICommandRequest
    {
        private readonly List<string> handlers;
        private readonly IDictionary<string, object> properties;

        protected BaseCommand()
        {
            handlers = new List<string>();
            properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        ~BaseCommand()
        {
            Dispose();
        }

        public bool ForwardForce { get; set; }

        protected override void DisposeCore()
        {
            handlers?.Clear();
            properties?.Clear();
        }

        public virtual IList<string> Handlers
        {
            get
            {
                return handlers;
            }
            set
            {
                if ((value == null) || (value.Count == 0))
                    handlers.Clear();
                else
                {
                    value.Each(s => handlers.Add(s));
                }
            }
        }

        public virtual IDictionary<string, object> Properties
        {
            get
            {
                return properties;
            }
            set
            {
                if ((value == null) || (value.Count == 0))
                    properties.Clear();
                else
                {
                    value.Each(kvp => properties.Add(kvp));
                }
            }
        }

        public CommandProcessor CommandProcessor { get; set; }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}