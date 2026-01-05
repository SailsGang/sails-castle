var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume() ;

var db = postgres.AddDatabase("sailsenergy");

var redis = builder.AddRedis("redis")
    .WithDataVolume()
    .WithRedisCommander();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume()
    .WithManagementPlugin();

// API
builder.AddProject<Projects.SailsEnergy_Api>("api")
    .WithReference(db)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(db)
    .WaitFor(redis)
    .WaitFor(rabbitmq);

builder.Build().Run();
