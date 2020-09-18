# StockMarketServiceApp
Stock Service Web API implemented in Asp.Net Core 3
<br >
StockMarket.Core project: Interfaces, Model classes, shared among all other projects
<br >
StockMarket.Api project: REST Api implementation with Authentication, SingalR Hub, Swagger document
<br >
StockMarket.Bll project: Business Logic Layer, implementing service to inject into Api controller 
<br >
StockMarket.Identity project: Authentication, JWT token
<br >
StockMarket.Data.Ef project: Data Access Layer, MS SQL Server + Entity Framework
<br >
StockMarket.Data.Dapper project: Data Access Layer, MS SQL Server + Dapper
<br >
StockMarket.Data.MongoDB project: Data Access Layer, MongoDB
<br >
StockMarketApi.Test project: Unit Test for controller, service and Dal repositories

<br >
In appsettings.json of StockMarket.Api project, set "DatabaseProvider" to configure Dal layer:
<br >
"SqlServerEf" for MS SqlServer, Entity Framework;
<br >
"SqlServerDapper" for MS SqlServer, Dapper
<br >
"MongoDB" for MongoDB
<br >




