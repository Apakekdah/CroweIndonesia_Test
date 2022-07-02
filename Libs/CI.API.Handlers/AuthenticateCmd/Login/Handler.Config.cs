namespace CI.API.Handlers.AuthenticateCmd.Login
{
    public partial class Handler
    {
        public class HandlerConfig : Config
        {
            public HandlerConfig(string name = null)
                : base((name ?? DEFAULT_NAME)) { }

            public int MaxFailed { get; set; }
        }
    }
}