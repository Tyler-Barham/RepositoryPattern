# RepositoryPattern

## About the project
___WIP__: This project needs a summary._


## Getting started
This is a `.net5.0` project. It depends on
* NewtonsoftJson v5.x.x
* MongoDB.Driver v2.x.x
* CassandraCSharpDriver v3.x.x
* NSwag v13.x.x
* xUnit v2.X.x

Then install the code coverage tool globally:
```sh
> dotnet tool install -g dotnet-reportgenerator-globaltool
```

### Running the API
To get it up and running via the command line:
```sh
> dotnet run --project ./BooksApi/BooksApi.csproj
# Navigate to localhost:5000/swagger
```

### Running the tests
To run tests via the command line:
```sh
> dotnet test --collect:"XPlat Code Coverage"

# Example output
Passed!  - Failed:     0, Passed:     6, Skipped:     1, Total:     7, Duration: 25 ms - BooksApi.UnitTests.dll (net5.0)

> reportgenerator -reports:"./BooksApi.UnitTests/TestResults/{guid}/coverage.cobertura.xml" -targetdir:"./BooksApi.UnitTests/TestResults/CoverageReport" -reporttypes:Html
# Now open the index.html that was generated in the target directory
```

## Databases
All databases use the Repository pattern and implement the [IRepository](BooksApi/Repository/IRepository.cs) interface.

### In-memory
There is an in-memory database, [RAMRepository](BooksApi/Repository/RAMRepository.cs), which is used for testing.

### MongoDB
There is a free cloud MongoDB instance hosted via Mongo Atlas in AWS' Sydney datacenter. The connection details are within [appsettings.json](BooksApi/appsettings.json) and built into a connection string within [MongoRepository.cs](BooksApi/Repository/MongoRepository.cs).

### Cassandra
There is a free cloud Cassandra instance hosted via Datastax AstraDB in Azures' Sydney datacenter. The connection details (i.e. certs) are within [this local zip](BooksApi/Repository/secure-connect-bookstoredb.zip), and user settings are within [appsettings.json](BooksApi/appsettings.json). These details are used in [CassandraRepository.cs](BooksApi/Repository/CassandraRepository.cs).
