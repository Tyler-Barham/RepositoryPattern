# RepositoryPattern

## About the project
___WIP__: This project needs a summary._


## Getting started
This is a `.net5.0` project. It depends on
* NewtonsoftJson v5.x.x
* MongoDB.Driver v2.x.x

To get it up and running via the command line:
```sh
> dotnet run
# Navigate to localhost:5001/api
```

## Databases

### MongoDB
There is a free cloud MongoDB instance hosted via Mongo Atlas in AWS' Sydney datacenter. The connection string is within [appsettings.json](BooksApi/appsettings.json) as `mongodb+srv://<user>:<password>@<cluster>/<database>?<parameters>`.

### Cassandra
Not yet implemented.