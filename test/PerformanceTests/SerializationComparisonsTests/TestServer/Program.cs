using ExperimentsLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace TestServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ServerCollection<IEnumerable<Customer>> writers = DefaultServerCollection.Create(null);
            builder.Services.AddSingleton(writers);
            builder.Services.AddControllers(options =>
            {
                options.OutputFormatters.Insert(0, new TestOutputFormatter());
            });

            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // the baseline uses AspNetCore's default serializer
            app.MapGet("/customers/baseline", ([FromQuery] int? count) =>
            {
                var data = CustomerDataSet.GetCustomers(count ?? 100);
                return data;
            });

            app.MapControllers();

            app.Run();
        }
    }
}