//cloud database implementation using MongoDB
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


//struct to group related stuff together
struct BookSummary
{
    // The fields are public so they can be set and read directly
    public string Title;
    public string Author;
    public double Price;


    //Booksummary uses Book class to create a summary of the book
    public BookSummary(string title, string author, double price)
    {
        Title = title;
        Author = author;
        Price = price;
    }

    // A method on the struct that returns a formatted string representation
    public string Display()
    {
        return $"Title: {Title} | Author: {Author} | Price: ${Price:F2}";
    }
}

//ignore fields it cant map
[BsonIgnoreExtraElements]
class Book
{
//switch to Bson attributes to map the class properties to the MongoDB document fields

//tells MongoDB that this property is the unique identifier for the document
  [BsonId]
  
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = "";

    [BsonElement("title")]  public string Title  { get; set; } = "";
    [BsonElement("author")] public string Author { get; set; } = "";
    [BsonElement("genre")]  public string Genre  { get; set; } = "";
    [BsonElement("price")]  public double Price  { get; set; }
    [BsonElement("stock")]  public int    Stock  { get; set; }

}