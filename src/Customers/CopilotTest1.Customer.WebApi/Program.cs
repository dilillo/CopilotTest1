using CopilotTest1.People.Data;
using Microsoft.EntityFrameworkCore;
using Orleans.Configuration;
using System.Net;

namespace CopilotTest1.People.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<PeopleDbContext>(options =>
               options.UseSqlServer(dbConnectionString));

            builder.Host.UseOrleans((context, builder) =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    builder
                        .UseLocalhostClustering()
                        .AddCustomStorageBasedLogConsistencyProviderAsDefault();
                }
                else
                {
                    var endpointAddress = IPAddress.Parse(context.Configuration["WEBSITE_PRIVATE_IP"]!);
                    var stringifiedPorts = context.Configuration["WEBSITE_PRIVATE_PORTS"]!.Split(',');

                    if (stringifiedPorts.Length < 2)
                    {
                        throw new Exception("Insufficient private ports configured.");
                    }

                    var (siloPort, gatewayPort) = (int.Parse(stringifiedPorts[0]), int.Parse(stringifiedPorts[1]));

                    var connectionString = context.Configuration["ORLEANS_AZURE_STORAGE_CONNECTION_STRING"];

                    builder
                        .ConfigureEndpoints(endpointAddress, siloPort, gatewayPort)
                        .Configure<ClusterOptions>(
                            options =>
                            {
                                options.ClusterId = context.Configuration["ORLEANS_CLUSTER_ID"];
                                options.ServiceId = "PeopleService";
                            })
                        .UseAdoNetClustering(
                            options =>
                            {
                                options.Invariant = "System.Data.SqlClient";
                                options.ConnectionString = dbConnectionString;
                            });
                }
            });

            var app = builder.Build();

            CreateDbIfNotExists(app);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<PeopleDbContext>();

                    context.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();

                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
    }
}