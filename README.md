# Overview

This project is a C# console bookstore app that connects to a **MongoDB Atlas** cloud database to perform full CRUD operations — view, add, update, and remove books. The connection string is securely loaded from a `.env` file at runtime. 

Key concepts practiced:
- Connecting to a cloud MongoDB Atlas database from C#
- Mapping C# classes to MongoDB documents using `[BsonId]` and `[BsonElement]` attributes
- Querying all documents from a collection with `Find` and `FilterDefinition<T>.Empty`
- Inserting documents with `InsertOne`
- Deleting documents by ObjectId with `DeleteOne` and a lambda filter
- Updating specific fields with `Builders<T>.Update.Set` and `UpdateOne`
- Loading secrets securely from a `.env` file
- Using `MongoDB.Driver` NuGet package


# Cloud Database

This project uses **MongoDB Atlas**, a cloud-hosted NoSQL document database. The connection string is stored in a `.env` file (excluded from source control via `.gitignore`) and loaded at runtime. The app connects using the official `MongoDB.Driver` NuGet package.

**Database:** `cse310`  
**Collection:** `bookstore`

Each document in the `bookstore` collection represents a book with the following structure:

| Field    | BSON Type | Description                        |
|----------|-----------|------------------------------------|
| `_id`    | ObjectId  | Auto-generated unique identifier   |
| `title`  | String    | Title of the book                  |
| `author` | String    | Author of the book                 |
| `genre`  | String    | Genre of the book                  |
| `price`  | Double    | Price of the book                  |
| `stock`  | Int32     | Number of copies in stock          |

Unlike SQLite, MongoDB does not require a schema or table creation — the collection and documents are created automatically on first insert. The C# `Book` class uses `[BsonId]` and `[BsonElement]` attributes to map properties to their corresponding MongoDB document fields.


# Development Environment

IDE: Visual Studio Code  
Programming Language: C#  
.NET SDK: .NET 10.0  
Operating System: Windows 10/11  
NuGet Package: `MongoDB.Driver` 3.7.1  
Recommended Extensions:
- C# for Visual Studio Code
- .NET CLI

# Useful Websites

* [Microsoft C# Docs](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/)
* [MongoDB Atlas](https://www.mongodb.com/atlas)
* [MongoDB C# Driver Docs](https://www.mongodb.com/docs/drivers/csharp/current/)
* [MongoDB BSON Serialization](https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/serialization/)
* [MongoDB Change Streams](https://www.mongodb.com/docs/manual/changeStreams/)
* [Youtube](https://www.youtube.com/watch?v=8bOoiftm5wM)

# Future Work

- Add additional collections (e.g. customers, orders) and link them by ObjectId references
- Add search/filter functionality (e.g. find books by genre or author)
- Implement user authentication
- Use MongoDB change streams to receive real-time notifications when data changes
