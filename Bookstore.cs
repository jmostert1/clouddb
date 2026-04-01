
using System;
using System.Data;
using Microsoft.Data.Sqlite;

class Bookstore
{

    //switched filepath to dbpath
    private string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "books.db");

    //create db if doesnt exist
    public void InitializeDatabase()
    {
        //opens our db and closes when its done
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        //our SQL to create the table
        string createTable = @"
            CREATE TABLE IF NOT EXISTS Books (
                Id      INTEGER PRIMARY KEY AUTOINCREMENT,
                Title   TEXT NOT NULL,
                Author  TEXT NOT NULL,
                Genre   TEXT NOT NULL,
                Price   REAL NOT NULL,
                Stock   INTEGER NOT NULL
            );";

        //execute our query for us
        using var command = new SqliteCommand(createTable, connection);
        //runs query, Nonquery means we dont want anything back
        command.ExecuteNonQuery();
    }


    public void SeedBooks()
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        // Check if table already has data â€” if so, skip seeding
        string countQuery = "SELECT COUNT(*) FROM Books;";
        using var countCommand = new SqliteCommand(countQuery, connection);
        long count = (long)countCommand.ExecuteScalar()!;

        if (count > 0) return; // Already has books, don't seed again

        string insert = @"
        INSERT INTO Books (Title, Author, Genre, Price, Stock) VALUES
        ('The Great Gatsby',                       'F. Scott Fitzgerald', 'Fiction',   10.99, 5),
        ('To Kill a Mockingbird',                  'Harper Lee',          'Fiction',   12.49, 3),
        ('1984',                                   'George Orwell',       'Dystopian',  9.99, 8),
        ('The Hobbit',                             'J.R.R. Tolkien',      'Fantasy',   14.99, 6),
        ('Harry Potter and the Sorcerer''s Stone', 'J.K. Rowling',        'Fantasy',   13.99, 10),
        ('The Da Vinci Code',                      'Dan Brown',           'Thriller',  11.49, 4),
        ('Brave New World',                        'Aldous Huxley',       'Dystopian', 10.49, 7),
        ('The Catcher in the Rye',                 'J.D. Salinger',       'Fiction',    9.49, 2),
        ('Pride and Prejudice',                    'Jane Austen',         'Romance',    8.99, 9),
        ('The Alchemist',                          'Paulo Coelho',        'Adventure', 11.99, 5);";

        using var command = new SqliteCommand(insert, connection);
        command.ExecuteNonQuery();

        Console.WriteLine("Books loaded into database.");
    }



    public void ViewBooks()
    {
        //same code as earlier, established connection
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        //our standard SELECT query
        string query = "SELECT Id, Title, Author, Genre, Price, Stock FROM Books;";

        //execute our query using SqliteCommand(action, connection)
        using var command = new SqliteCommand(query, connection);

        using var reader = command.ExecuteReader();

        Console.WriteLine("Books in the Store:");

        //read through the reader object row by row. values have to match the select query above, so we know the order of the columns
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string title = reader.GetString(1);
            string author = reader.GetString(2);
            string genre = reader.GetString(3);
            double price = reader.GetDouble(4);
            int stock = reader.GetInt32(5);


            //same code before to now display from db instead of from the json.
            BookSummary summary = new BookSummary(title, author, price);
            Console.WriteLine($"[ID: {id}] Genre: {genre} | Stock: {stock}");
            Console.WriteLine($"  {summary.Display()}");
        }


    }

    public void AddBook()
    {
        //Code to add a book
        //ask user for book details
        Console.Write("Enter book title: ");
        string title = Console.ReadLine()!;
        Console.Write("Enter book author: ");
        string author = Console.ReadLine()!;
        Console.Write("Enter book genre: ");
        string genre = Console.ReadLine()!;
        Console.Write("Enter book price: ");
        double price = Convert.ToDouble(Console.ReadLine()!);
        Console.Write("Enter book stock: ");
        int stock = Convert.ToInt32(Console.ReadLine()!);


        //same code as earlier, established connection
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();


        //our insert query, @ symbol means exactly
        string query = @"
        INSERT INTO Books (Title, Author, Genre, Price, Stock)
        VALUES (@title, @author, @genre, @price, @stock);";

        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@title", title);
        command.Parameters.AddWithValue("@author", author);
        command.Parameters.AddWithValue("@genre", genre);
        command.Parameters.AddWithValue("@price", price);
        command.Parameters.AddWithValue("@stock", stock);

        command.ExecuteNonQuery();
        Console.WriteLine("Book added!");




    }


    public void Removebook()
    {
        //Code to remove a book
        //ask user which book to remove
        Console.Write("Enter the ID of the book to remove: ");
        int idToRemove = Convert.ToInt32(Console.ReadLine()!);


        //same code as earlier, established connection
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        string query = "DELETE FROM Books WHERE Id = @id;";
        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@id", idToRemove);

        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected > 0)
            Console.WriteLine("Book removed successfully!");
        else
            Console.WriteLine("No book found with that ID.");
    }


    public void UpdateBook()
    {
        // Show the current books so the user knows which ID to pick
        ViewBooks();

        //grab the id we want to update
        Console.Write("Enter the ID of the book to update: ");
        int idToUpdate = Convert.ToInt32(Console.ReadLine()!);

        Console.Write("Enter new price (or press Enter to keep current): ");
        string? priceInput = Console.ReadLine();

        Console.Write("Enter new stock (or press Enter to keep current): ");
        string? stockInput = Console.ReadLine();

        // Build the SET clause dynamically based on what the user provided
        var setClauses = new List<string>();
        if (!string.IsNullOrWhiteSpace(priceInput)) setClauses.Add("Price = @price");
        if (!string.IsNullOrWhiteSpace(stockInput)) setClauses.Add("Stock = @stock");

        if (setClauses.Count == 0)
        {
            Console.WriteLine("No changes made.");
            return;
        }


        //establish connection
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        // Combine the SET clauses into one UPDATE query
        // UPDATE Books SET Price = @price, Stock = @stock WHERE Id = @id;
        string query = $"UPDATE Books SET {string.Join(", ", setClauses)} WHERE Id = @id;";

        using var command = new SqliteCommand(query, connection);

        //users parameters for safety, only add parameters for the fields they want to update
        command.Parameters.AddWithValue("@id", idToUpdate);
        if (!string.IsNullOrWhiteSpace(priceInput))
            command.Parameters.AddWithValue("@price", Convert.ToDouble(priceInput));
        if (!string.IsNullOrWhiteSpace(stockInput))
            command.Parameters.AddWithValue("@stock", Convert.ToInt32(stockInput));


        //doesnt return anything
        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected > 0)
            Console.WriteLine("Book updated successfully!");
        else
            Console.WriteLine("No book found with that ID.");
    }


    public void ViewStats()
    {
        //same as before established connection
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        // Use aggregate functions to summarize the inventory
        //make our query
        string query = @"
            SELECT
                COUNT(*)    AS TotalBooks,
                AVG(Price)  AS AvgPrice,
                SUM(Stock)  AS TotalStock,
                MIN(Price)  AS CheapestPrice,
                MAX(Price)  AS MostExpensivePrice
            FROM Books;";

        //execute our query using SqliteCommand(action, connection)
        using var command = new SqliteCommand(query, connection);

        //reader, we want something back
        using var reader = command.ExecuteReader();

        // Read the results and display the stats
        if (reader.Read())
        {
            int    totalBooks    = reader.GetInt32(0);
            double avgPrice      = reader.GetDouble(1);
            int    totalStock    = reader.GetInt32(2);
            double cheapest      = reader.GetDouble(3);
            double mostExpensive = reader.GetDouble(4);

            Console.WriteLine("\n--- Inventory Stats ---");
            Console.WriteLine($"Total unique titles : {totalBooks}");
            Console.WriteLine($"Total books in stock: {totalStock}");
            Console.WriteLine($"Average price       : ${avgPrice:F2}");
            Console.WriteLine($"Cheapest book       : ${cheapest:F2}");
            Console.WriteLine($"Most expensive book : ${mostExpensive:F2}");
        }
    }


}