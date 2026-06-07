var builder = DistributedApplication.CreateBuilder(args);

var aks = builder.AddAzureKubernetesEnvironment("aks");

var cache = builder.AddRedis("cache");

// Add SQL Server
var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume();

var database = sqlServer.AddDatabase("aspireappdb");

// Add migration service to run migrations
var migrationService = builder.AddProject<Projects.AspireApp_MigrationService>("migrationservice")
    .WithReference(database)
    .WaitFor(sqlServer);

var apiService = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
    .WithReference(database)
    .WithReference(cache)
    .WaitFor(database)
    .WaitFor(migrationService);

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService)
    .WaitFor(cache)
    .WaitFor(apiService);

builder.Build().Run();
