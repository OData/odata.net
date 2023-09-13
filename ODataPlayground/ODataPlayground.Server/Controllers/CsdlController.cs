namespace Portal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Microsoft.Graph.Onboarding.DocsGenerator.Utils;
    using Microsoft.Identity.Web;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm;
    using Microsoft.TeamFoundation.SourceControl.WebApi;
    using Microsoft.VisualStudio.Services.Common;
    using System.IO.Compression;
    using System.Security.Permissions;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Text.RegularExpressions;
    using System.Xml;
    using static System.Runtime.InteropServices.JavaScript.JSType;
    using Microsoft.Graph.Studio;
    using System.IO;
    using Microsoft.Graph.AGS.Schema;
    using Portal.Core.CsdlStorage;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System;

    public static class Extensions
    {
        public static (T first, IEnumerable<T> theRest) EnumerateFirst<T>(this IEnumerable<T> self)
        {
            return (self.First(), self.Skip(1));
        }

        public static IEnumerable<TSource> Apply<TSource, TResult>(this (TSource first, IEnumerable<TSource> theRest) self, Func<TSource, TResult> selector, out TResult first)
        {
            first = selector(self.first);
            return self.theRest;
        }

        public static (T first, IEnumerator<T> theRest) EnumerateFirst2<T>(this IEnumerable<T> self)
        {
            using (var enumerator = self.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new Exception("TODO");
                }

                return (enumerator.Current, enumerator);
            }

            ////return (self.First(), self.Skip(1));
        }
    }

    [ApiController]
    [Route("csdls")]
    public sealed class CsdlController : ControllerBase
    {
        private readonly ICsdlStorage csdlStorage;

        private readonly string adoPersonalAccessToken;

        private readonly bool downloadWorkloads;

        public CsdlController(ICsdlStorage csdlStorage, IConfiguration configuration)
        {
            this.csdlStorage = csdlStorage;
            this.adoPersonalAccessToken = configuration["WorkloadsStorage:PersonalAccessToken"] ?? throw new InvalidOperationException("No ADO personal access token was specified in the configuration");
            this.downloadWorkloads = bool.Parse(configuration["WorkloadsStorage:DownloadWorkloads"] ?? throw new InvalidOperationException("No ADO personal access token was specified in the configuration"));
        }

        [HttpPut]
        public async Task<IActionResult> Put([ValidateNever][FromBody] string data)
        {
            var personalAccessToken = this.adoPersonalAccessToken;
            var workloadRegex = new Regex(@"Workloads/(?<workload>Microsoft.*)/override/schema-Prod-beta\.csdl", RegexOptions.IgnoreCase); //// TODO parameterize

            //// TODO error handling this is pretty important

            string finalCsdl;
            if (!this.downloadWorkloads)
            {
                finalCsdl = Encoding.UTF8.GetString(Convert.FromBase64String(data));
            }
            else
            {
                using (var connection = new Microsoft.VisualStudio.Services.WebApi.VssConnection(new Uri("https://msazure.visualstudio.com"), new VssBasicCredential(string.Empty, personalAccessToken))) //// TODO parameterize
                {
                    using (var client = connection.GetClient<GitHttpClient>())
                    {
                        using (var workloadsStream = await client.GetItemZipAsync( //// TODO parameterize
                            project: "One",
                            repositoryId: "AD-AggregatorService-Workloads",
                            path: "/Workloads",
                            includeContent: true,
                            versionDescriptor: new GitVersionDescriptor
                            {
                                VersionType = GitVersionType.Branch,
                                Version = "master",
                            }))
                        {
                            using (var zipArchive = new ZipArchive(workloadsStream))
                            {
                                var schemaEntries = zipArchive
                                    .Entries
                                    .Select(entry => (entry, match: workloadRegex.Match(entry.FullName)))
                                    .Where(e => e.match.Success)
                                    .Select(e =>
                                    {
                                        var workloadName = e.match.Groups[1].Value;
                                        var stream = new MemoryStream();
                                        e.entry.Open().CopyTo(stream);

                                        return (workloadName, stream);
                                    });
                                ////.Append((workloadName: "Microsoft.Playground", stream: new MemoryStream(Encoding.UTF8.GetBytes(data)))); TODO
                                var schemaLoader = new Microsoft.Graph.AGS.Schema.InternalSchemaLoader(new SchemaServiceConfigurations(), new GlobalFeatureProvider());
                                InternalSchema internalSchema;
                                int thing = 1;
                                foreach (var entry in schemaEntries
                                    .EnumerateFirst()
                                    .Apply(
                                        first =>
                                        {
                                            var model = GetEdmModel(first.stream);
                                            return schemaLoader.LoadSchema(model, first.workloadName, (GraphVersionFlags)int.MaxValue);//// Tags.Parse("asdf")); //// TODO nul shuold be graph flags
                                        },
                                        out internalSchema))
                                {
                                    var model = GetEdmModel(entry.stream);
                                    schemaLoader.LoadSchema(model, internalSchema, entry.workloadName, (GraphVersionFlags)int.MaxValue);//// Tags.Parse("asdf")); //// TODO tags
                                    ++thing;
                                }

                                internalSchema.GenerateModel(null); //// TODO tags set
                                using (var stream = new MemoryStream())
                                {
                                    using (var xmlWriter = XmlWriter.Create(stream))
                                    {
                                        CsdlWriter.TryWriteCsdl(internalSchema.Model, xmlWriter, CsdlTarget.OData, out var errors);

                                    }

                                    stream.Position = 0;
                                    finalCsdl = Encoding.UTF8.GetString(stream.ToArray());
                                }
                            }
                        }
                    }
                }
            }

            //// Microsoft.Graph.AGS.Schema.InternalSchemaLoader
            //// Microsoft.VisualStudio.Services.WebApi.VssConnection
            //// TODO add the ags csdls to this
            //// https://microsoftgraph.visualstudio.com/onboarding/_git/playground-service?path=/Playground/Services/DataSourceProvider.cs&version=GBmain&line=146&lineEnd=146&lineStartColumn=39&lineEndColumn=54&lineStyle=plain&_a=contents
            var identifier = await this.csdlStorage.PutAsync(Encoding.UTF8.GetBytes(finalCsdl));
            var location = $"https://{HttpContext.Request.Host}/csdls/{identifier}";
            Response.Headers.Add("Location", location);
            return Ok();
        }

        private static IEdmModel GetEdmModel(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = XmlReader.Create(stream);
            if (CsdlReader.TryParse(reader, out var model, out var errors))
            {
                return model;
            }

            throw new InvalidOperationException(string.Concat(errors.Select(e => e.ErrorMessage)));
        }

        [HttpGet]
        [Route("{csdlIdentifier}")]
        public async Task<IActionResult> Get([FromRoute] string csdlIdentifier)
        {
            var csdl = await this.csdlStorage.GetAsync(csdlIdentifier);
            if (!this.downloadWorkloads)
            {
                csdl = Encoding.UTF8.GetBytes(Convert.ToBase64String(csdl));
            }

            return Ok(Encoding.UTF8.GetString(csdl));
        }
    }
}
