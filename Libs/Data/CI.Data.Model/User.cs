using Ride.Attributes.DB;

namespace CI.Data.Entity
{
    [TableName("td_User")]
    public class User
    {
        [FieldKey]
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public int Counter { get; set; }
    }
}