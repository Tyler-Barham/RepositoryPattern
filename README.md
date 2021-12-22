# RepositoryPattern

## About the project
This is a smaple project with the aim of implementing MongoDB and Cassandra as repositories that can be swapped in/out with one another.


### Databases
All databases use the Repository pattern and implement the [IRepository](BooksApi/Repositories/IRepository.cs) interface.

The types are:
* __In-memory:__ There is an in-memory database, which is used for testing.
* __MongoDB:__ There is a free cloud MongoDB instance hosted via Mongo Atlas in AWS' Sydney datacenter.
* __Cassandra:__ There is a free cloud Cassandra instance hosted via Datastax AstraDB in Azures' Sydney datacenter.


## Getting started
This is a `.net5.0` project. It depends on
* CassandraCSharpDriver v3.17.x
* MongoDB.Driver v2.14.x
* NewtonsoftJson v5.0.x
* NSwag v13.14.x
* xUnit v2.4.x

To get started:
* Run the following to download the above dependancies:
  ```sh
  > dotnet restore
  ```
* Get database instances running
  * Signup for free cloud instances of [MongoDB](https://account.mongodb.com/account/register) or [Cassandra (via Datastax)](https://astra.datastax.com/register).
  * Or host MongoDB/Cassandra yourself.
* If using Cassandra, create your table (example code within the [CassandraRepository.cs](https://github.com/Tyler-Barham/RepositoryPattern/blob/bce84150b05d4bbc79149ff2205b58ada2490f23/BooksApi/Repositories/CassandraRepository.cs#L65))
* Update the database credentials in [appsettings.Development.json](BooksApi/appsettings.Development.json)


### Running the API
To get it up and running via the command line:
```sh
> dotnet run --project .\BooksApi\BooksApi.csproj
# Navigate to localhost:5000/swagger
```

### Running the tests
To run tests via the command line:
```sh
> dotnet test -s .\BooksApi.UnitTests\test.runsettings
# Example output
Passed!  - Failed:     0, Passed:     6, Skipped:     1, Total:     7, Duration: 25 ms - BooksApi.UnitTests.dll (net5.0)
```

To run the tests and generate code coverage:

```sh
# Install the code coverage tool
> dotnet tool install -g dotnet-reportgenerator-globaltool

# Run the script
> cd .\BooksApi.UnitTests\
> .\GenCodeCoverage.ps1
# Example output
   Line coverage:  30.9% (115 of 371)
   Branch coverage:  27% (20 of 74)
   Method coverage:  26.6% (28 of 105)

# Now open .\TestResults\CoverageReport\index.html to view a detailed breakdown of coverage
```
