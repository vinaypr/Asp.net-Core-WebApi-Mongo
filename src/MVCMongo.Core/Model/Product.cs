namespace MVCMongo.Core.Model
{
    using MongoDB.Bson;

    public class Product
    {
        public ObjectId Id { get; set; } // This is from MongoDB.Bason it is for Mapping CLR object to Mongo Collection

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int Price { get; set; }

        public string Category { get; set; }
    }
}
