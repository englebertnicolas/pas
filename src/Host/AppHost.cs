using PAS.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var dbCnc = builder.AddConnectionString("Database");
var dbMigrator = builder.AddProject<Projects.PAS_DbMigrator>("pas-dbmigrator")
    .WithReference(dbCnc);

var rabbitMq = builder.AddRabbitMQ("RabbitMq")
    .WithManagementPlugin();

builder.AddProject<Projects.PAS_Assets_Api>("pas-assets-api")
    .WithReference(dbCnc)
    .WithReference(rabbitMq)
    .WithScalarEndpoint()
    .WaitForCompletion(dbMigrator)
    .WaitFor(rabbitMq);

builder.AddProject<Projects.PAS_Policies_Api>("pas-policies-api")
    .WithReference(dbCnc)
    .WithReference(rabbitMq)
    .WithScalarEndpoint()
    .WaitForCompletion(dbMigrator)
    .WaitFor(rabbitMq);

builder.Build().Run();
