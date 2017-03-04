namespace MVCMongo.Core.Model
{
    using MongoDB.Bson;

    public class User
    {
        public ObjectId Id { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
