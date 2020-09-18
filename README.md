# StockMarketServiceApp
Stock Service Web API implemented in Asp.Net Core 3

StockMarket.Core project: Interfaces, Model classes, shared among all other projects
StockMarket.Api project: REST Api implementation with Authentication, SingalR Hub, Swagger document
StockMarket.Bll project: Business Logic Layer, implementing service to inject into Api controller 
StockMarket.Identity project: Authentication, JWT token
StockMarket.Data.Ef project: Data Access Layer, MS SQL Server + Entity Framework
StockMarket.Data.Dapper project: Data Access Layer, MS SQL Server + Dapper
StockMarket.Data.MongoDB project: Data Access Layer, MongoDB
StockMarketApi.Test project: Unit Test for controller, service and Dal repositories

In appsettings.json of StockMarket.Api project, set "DatabaseProvider" to configure Dal layer:
"SqlServerEf" for MS SqlServer, Entity Framework;
"SqlServerDapper" for MS SqlServer, Dapper
"MongoDB" for MongoDB




