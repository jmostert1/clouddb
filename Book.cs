

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

class Book
{
    public int id { get; set; }
    public string title { get; set; } = "";
    public string author { get; set; } = "";
    public string genre { get; set; } = "";
    public double price { get; set; }
    public int stock { get; set; }

}