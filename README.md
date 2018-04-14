# WeatherService - README

**WeatherService** lets you collect, share and organize measurements of
weather stations. A station can be public available or only for registered
users.

## Installation

### Checkout source code

Check out the source code with the following command:

```
$ git clone https://github.com/20centaurifux/WeatherService
```

### Install dependencies

The solution is based on .NET Core 2. To install dependencies go to the
previously created "WeatherService" directory and run these commands:

```
$ dotnet restore
$ cd WeatherStation
$ bower install
```

### Setup database

**WeatherService** uses [LINQ to DB](https://github.com/linq2db/linq2db) for database access. You find scripts for
creating a new Firebird or SQLite database in the [SQL](https://github.com/20centaurifux/WeatherService/tree/master/SQL) subfolder.

Here's an example for creating a new SQLite database:

```
$ sqlite3 database/WeatherService.db < SQL/sqlite.sql
```

After setting up the database define the connection string and database provider
in the [configuration file](https://github.com/20centaurifux/WeatherService/blob/master/WeatherService/configuration/WeatherService.ini).

### Create administrator account

At first start the web application.

```
$ dotnet run
```

Now visit the website below to create the administrator account.

```
http://localhost:5000/Setup
```

Login and change the password under "My Account" -> "My Profile".


