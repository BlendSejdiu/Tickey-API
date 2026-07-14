var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Tickey>("tickey");

builder.Build().Run();
