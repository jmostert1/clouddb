// See https://aka.ms/new-console-template for more information

Console.WriteLine($"Welcome to JPM Bookstore");

Bookstore bookstore = new Bookstore();
bookstore.InitializeDatabase();
bookstore.SeedBooks();
string? option = "";


do
{
    Console.WriteLine();
    Console.Write("Options: \n1. View Books \n2. Add Book \n3. Remove Book \n4. Update Book \n5. View Stats \n6. Exit\n");
    Console.WriteLine();
    Console.Write("Please select an option: ");
    option = Console.ReadLine();

    if (option == "1")
    {
        Console.WriteLine("Viewing Books...");
        // Code to display books
        bookstore.ViewBooks();
    }
    else if (option == "2")
    {
        Console.WriteLine("Adding a Book...");
        // Code to add a book
        bookstore.AddBook();
    }
    else if (option == "3")
    {
        Console.WriteLine("Removing a Book...");
        // Code to remove a book
        bookstore.Removebook();
    }
    else if (option == "4")
    {
        Console.WriteLine("Updating a Book...");
        bookstore.UpdateBook();
    }
    else if (option == "5")
    {
        Console.WriteLine("Viewing Stats...");
        bookstore.ViewStats();
    }
    else if (option == "6")
    {
        Console.WriteLine("Exiting...");
    }
    else
    {
        Console.WriteLine("Invalid option. Please try again.");
    }

} while (option != "6");

////////////////////////////////////////////////////////////////////////////////////////////////////
// All of these basic requirements:
// You must create a database for your program to use including at least one table to store data into.

// Your software must demonstrate the ability to insert, modify, delete, and retrieve (or query) data.

// This module requires more than just creating the database and determining SQL commands. You must write software that builds the SQL commands, submits them, receives the results from the database, and uses the results in some way.

//////////////////////////////////////////////////////////////////////////////////////////////////////
// One of these additional requirements:

// Add additional tables to your database and perform a join in your software between two of the tables.

// Use at least two of the aggregate functions to summarize numerical data in your database.

// Demonstrate the use of a column containing a date or time along with a query that demonstrates filtering within a date or time range.