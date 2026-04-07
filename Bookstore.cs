

using MongoDB.Driver;
using MongoDB.Bson;

class Bookstore
{
    // MongoDB collection to store books
    private readonly IMongoCollection<Book> _books;


    //Constructor
    public Bookstore()
    {
        //connect to MongoDB
        string connectionString = LoadConnectionString();
        var client   = new MongoClient(connectionString);


        //database name is cse310 and collection name is bookstore
        var database = client.GetDatabase("cse310");
        _books = database.GetCollection<Book>("bookstore");
    }


    

    private string LoadConnectionString()
    {
        //grab connection via the .env


        // Look for .env file in both the current directory and the base directory
        string[] paths = {
            Path.Combine(Directory.GetCurrentDirectory(), ".env"),
            Path.Combine(AppContext.BaseDirectory, ".env")
        };

        // Read the .env file and extract the MONGODB_URI value
        foreach (var path in paths)
        {
            if (!File.Exists(path)) continue;
            foreach (var line in File.ReadAllLines(path))
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("MONGODB_URI="))

                // Return the connection string without the "MONGODB_URI=" prefix
                    return trimmed.Substring("MONGODB_URI=".Length).Trim();
            }
        }
        throw new InvalidOperationException("MONGODB_URI not found in .env file.");
    }

  

    public void ViewBooks()
    {

        // Fetch all books from the MongoDB collection and display them (using .Find)
        //FilterDefinitetion = filter condition
        var books = _books.Find(FilterDefinition<Book>.Empty).ToList();
        Console.WriteLine("Books in the Store:");

        // Use the BookSummary struct to display a summary of each book
        for (int i = 0; i < books.Count; i++)
        {
            // Create a BookSummary instance for the current book
            var summary = new BookSummary(books[i].Title, books[i].Author, books[i].Price);

            // Display the book's genre and stock, followed by the summary
            Console.WriteLine($"[{i + 1}] Genre: {books[i].Genre} | Stock: {books[i].Stock}");
            Console.WriteLine($"  {summary.Display()}");
        }
    }

    public void AddBook()
    {
        //user input for book details
        Console.Write("Enter book title: ");  string title  = Console.ReadLine()!;
        Console.Write("Enter book author: "); string author = Console.ReadLine()!;
        Console.Write("Enter book genre: ");  string genre  = Console.ReadLine()!;
        Console.Write("Enter book price: ");  double price  = Convert.ToDouble(Console.ReadLine()!);
        Console.Write("Enter book stock: ");  int    stock  = Convert.ToInt32(Console.ReadLine()!);

        // Insert the new book into the MongoDB collection using .InsertOne
        _books.InsertOne(new Book { Title = title, Author = author, Genre = genre, Price = price, Stock = stock });
        Console.WriteLine("Book added!");
    }

    public void RemoveBook()
    {
        // Display books and let user select which one to remove
        var books = _books.Find(FilterDefinition<Book>.Empty).ToList();
        if (books.Count == 0) { Console.WriteLine("No books found."); return; }

        // Display books with indices for selection
        for (int i = 0; i < books.Count; i++)
            Console.WriteLine($"[{i + 1}] {books[i].Title} by {books[i].Author}");


        //ask which book to remove and validate input
        Console.Write("Enter the number of the book to remove: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > books.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        // Remove the selected book from the MongoDB collection using .DeleteOne
        //use a lamda expression b => b.Id == books[index - 1].Id
        var result = _books.DeleteOne(b => b.Id == books[index - 1].Id);
        Console.WriteLine(result.DeletedCount > 0 ? "Book removed successfully!" : "No book found.");
    }

    public void UpdateBook()
    {
        //show books and let user select which one to update
        ViewBooks();
        var books = _books.Find(FilterDefinition<Book>.Empty).ToList();


        //ask which book to update and validate input
        Console.Write("Enter the number of the book to update: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > books.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }


        //ask which fields to update and build the update definition
        Console.Write("Enter new price (or press Enter to keep current): ");
        string? priceInput = Console.ReadLine();
        Console.Write("Enter new stock (or press Enter to keep current): ");
        string? stockInput = Console.ReadLine();

        // Build a list of update definitions based on user input
        var updates = new List<UpdateDefinition<Book>>();
        if (!string.IsNullOrWhiteSpace(priceInput)) updates.Add(Builders<Book>.Update.Set(b => b.Price, Convert.ToDouble(priceInput)));
        if (!string.IsNullOrWhiteSpace(stockInput)) updates.Add(Builders<Book>.Update.Set(b => b.Stock, Convert.ToInt32(stockInput)));

        if (updates.Count == 0) { Console.WriteLine("No changes made."); return; }

        // Combine the update definitions and apply them to the selected book in the MongoDB collection
        var result = _books.UpdateOne(b => b.Id == books[index - 1].Id, Builders<Book>.Update.Combine(updates));
        Console.WriteLine(result.ModifiedCount > 0 ? "Book updated successfully!" : "No book found.");
    }

    public void ViewStats()
    {

        // Fetch all books from the MongoDB collection to calculate statistics
        var books = _books.Find(FilterDefinition<Book>.Empty).ToList();
        if (books.Count == 0) { Console.WriteLine("No books found."); return; }

        // Display various statistics about the inventory, such as total unique titles, total stock, average price, cheapest and most expensive books
        //F2 formats the price to 2 decimal places
        //lamda expressions b => b.Stock
        Console.WriteLine("\n--- Inventory Stats ---");
        Console.WriteLine($"Total unique titles : {books.Count}");
        Console.WriteLine($"Total books in stock: {books.Sum(b => b.Stock)}");
        Console.WriteLine($"Average price       : ${books.Average(b => b.Price):F2}");
        Console.WriteLine($"Cheapest book       : ${books.Min(b => b.Price):F2}");
        Console.WriteLine($"Most expensive book : ${books.Max(b => b.Price):F2}");
    }

    //Additional Feature: Real-time Change Notifications
    //MongoDb supports Change Streams which is a built in feature thats give notifications for data changes
    // Real-time change stream listener to display notifications when books are added, updated, or removed from the MongoDB collection  
    private readonly CancellationTokenSource _cts = new();

    // Start a background task to listen for changes in the MongoDB collection and display notifications in the console when changes occur  
    public void StartChangeStreamListener()
    {

        //documentation
        //https://www.mongodb.com/docs/manual/changeStreams/
        var options = new ChangeStreamOptions
        {
            FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
        };

        Task.Run(() =>
        {
            try
            {
                using var cursor = _books.Watch(options, _cts.Token);
                foreach (var change in cursor.ToEnumerable(_cts.Token))
                {
                    string message = change.OperationType switch
                    {
                        ChangeStreamOperationType.Insert =>
                            $"\n[NOTIFICATION] Book added: \"{change.FullDocument?.Title}\"",
                        ChangeStreamOperationType.Update =>
                            $"\n[NOTIFICATION] Book updated: \"{change.FullDocument?.Title}\"",
                        ChangeStreamOperationType.Delete =>
                            $"\n[NOTIFICATION] Book removed (ID: {change.DocumentKey["_id"]})",
                        _ => $"\n[NOTIFICATION] Change detected: {change.OperationType}"
                    };
                    Console.WriteLine(message);
                }
            }
            catch (OperationCanceledException) { }
        });

        Console.WriteLine("[INFO] Listening for real-time database changes...");
    }

    public void StopChangeStreamListener() => _cts.Cancel();
}