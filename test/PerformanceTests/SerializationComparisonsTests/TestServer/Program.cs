//---------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using ExperimentsLib;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace TestServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            WriterCollection<IEnumerable<Customer>> writers = DefaultWriterCollection.Create();
            builder.Services.AddSingleton(writers);
            builder.Services.AddControllers(options =>
            {
                options.OutputFormatters.Insert(0, new CustomerCollectionOutputFormatter());
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