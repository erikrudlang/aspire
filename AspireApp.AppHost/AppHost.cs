var builder = DistributedApplication.CreateBuilder(args);

var aks = builder.AddAzureKubernetesEnvironment("aks");

var cache = builder.AddRedis("cache");

var apiService = builder.AddContainer("apiservice", "aspireapp-apiservice")
    .WithDockerfile("../AspireApp.ApiService")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http")
    .WithHttpsEndpoint(port: 8081, targetPort: 8081, name: "https")
    .WithHttpHealthCheck("/health");

builder.AddContainer("webfrontend", "aspireapp-web")
    .WithDockerfile("../AspireApp.Web")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http")
    .WithHttpsEndpoint(port: 8081, targetPort: 8081, name: "https")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithEnvironment("services__apiservice__http__0", apiService.GetEndpoint("http"))
    .WithEnvironment("services__apiservice__https__0", apiService.GetEndpoint("https"))
    .WaitFor(apiService);

builder.Build().Run();
