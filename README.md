# RepositoryPattern

## About the project
___WIP__: This project needs a summary._


## Getting started
This is a `.net5.0` project. It depends on
* NewtonsoftJson v5.x.x
* MongoDB.Driver v2.x.x
* CassandraCSharpDriver v3.x.x
* NSwag v13.x.x

To get it up and running via the command line:
```sh
> dotnet run --project .\BooksApi\BooksApi.csproj
# Navigate to localhost:5000/swagger
```

## Databases

### MongoDB
There is a free cloud MongoDB instance hosted via Mongo Atlas in AWS' Sydney datacenter. The connection details are within [appsettings.json](BooksApi/appsettings.json) and built into a connection string within [MongoRepository.cs](BooksApi/Repository/MongoRepository.cs).

### Cassandra
There is a free cloud Cassandra instance hosted via Datastax AstraDB in Azures' Sydney datacenter. The connection details (i.e. certs) are within [this local zip](BooksApi/Repository/secure-connect-bookstoredb.zip), and user settings are within [appsettings.json](BooksApi/appsettings.json). These details are used in [CassandraRepository.cs](BooksApi/Repository/CassandraRepository.cs).