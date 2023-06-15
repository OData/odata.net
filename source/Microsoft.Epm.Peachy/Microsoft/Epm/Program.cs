namespace Microsoft.Epm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.HttpServer;
    using Microsoft.OData.ODataService;

    public static class Program //// TODO public for tests only?
    {
        public static async Task Main(string[] args) //// TODO public for tests only?
        {
            var assembly = Assembly.GetExecutingAssembly();
            var csdlResourceName = assembly.GetManifestResourceNames().Where(name => name.EndsWith("epm.csdl")).Single();
            using (var csdlResourceStream = assembly.GetManifestResourceStream(csdlResourceName))
            {
                var peachySettings = new Peachy.Settings()
                {
                    FeatureGapOData = new Epm(),
                };
                var epm = new Peachy(csdlResourceStream, peachySettings); //// TODO does webapi require running tests synchronously

                var port = 8080;
                await new HttpListenerHttpServer(
                    epm.HandleRequestAsync,
                    new HttpListenerHttpServer.Settings()
                    {
                        Port = port,
                    })
                    .ListenAsync();
            }
        }

        private sealed class Epm : IODataService
        {
            private class AuthorizationSystem
            {
                public string id { get; set; }

                public string authorizationSystemName { get; set; }

                public string authorizationSystemType { get; set; }

                public AssociatedIdentities associatedIdentities { get; set; } = new AssociatedIdentities();

                public class AssociatedIdentities
                {
                    public IReadOnlyDictionary<string, AuthorizationSystemIdentity> all { get; set; } = new Dictionary<string, AuthorizationSystemIdentity>();
                }
            }

            private readonly IReadOnlyDictionary<string, AuthorizationSystem> authorizationSystems;

            private class AuthorizationSystemIdentity
            {
                public string id { get; set; }

                public string displayName { get; set; }

                //// TODO public AuthorizationSystem authorizationSystem { get; set; }
            }

            private class AwsAuthorizationSystemIdentity : AuthorizationSystemIdentity
            {
            }

            private class AwsUser : AwsAuthorizationSystemIdentity
            {
                public IReadOnlyDictionary<string, AwsRole> assumableRoles { get; set; } = new Dictionary<string, AwsRole>();
            }

            private class AwsRole : AwsAuthorizationSystemIdentity
            {
            }

            public Epm()
            {
                var role = new AwsRole()
                {
                    id = "third",
                    displayName = "a role",
                };
                this.authorizationSystems = new[]
                {
                    new AuthorizationSystem()
                    {
                        id = "1",
                        authorizationSystemName = "chrispre auth system",
                        authorizationSystemType = "aws",
                        associatedIdentities = new AuthorizationSystem.AssociatedIdentities()
                        {
                            all = new[]
                            {
                                new AwsUser()
                                {
                                    id = "first",
                                    displayName = "adam",
                                    assumableRoles = new[]
                                    {
                                        role,
                                    }.ToDictionary(_ => _.id),
                                },
                                new AuthorizationSystemIdentity()
                                {
                                    id = "second",
                                    displayName = "jessie",
                                },
                                role,
                            }.ToDictionary(_ => _.id),
                        },
                    },
                    new AuthorizationSystem()
                    {
                        id = "2",
                        authorizationSystemName = "mikep auth system",
                        authorizationSystemType = "azure",
                    },
                    new AuthorizationSystem()
                    {
                        id = "3",
                        authorizationSystemName = "gdebruin auth system",
                        authorizationSystemType = "gcp",
                    },
                }.ToDictionary(element => element.id);
            }

            public async Task<ODataResponse> GetAsync(ODataRequest request)
            {
                var odataUri = new Uri($"http://testing{request.Url}", UriKind.Absolute);

                using (var segmentsEnumerator = odataUri.Segments.Select(segment => segment.Trim('/')).GetEnumerator())
                {
                    if (!segmentsEnumerator.MoveNext() || !segmentsEnumerator.MoveNext())
                    {
                        throw new InvalidOperationException("TODO no segments");
                    }

                    var path = new List<string>();
                    var segment = segmentsEnumerator.Current;
                    if (string.Equals(segment, "external"))
                    {
                        path.Add(segment);
                        if (!segmentsEnumerator.MoveNext())
                        {
                            return new ODataResponse(
                                200,
                                Enumerable.Empty<string>(),
                                GenerateStream(new
                                {
                                }));
                        }

                        segment = segmentsEnumerator.Current;
                        if (string.Equals(segment, "authorizationSystems"))
                        {
                            path.Add(segment);
                            var authorizationSystemSelector = (AuthorizationSystem _) => new { _.id, _.authorizationSystemName, _.authorizationSystemType };
                            if (segmentsEnumerator.MoveNext())
                            {
                                var authorizationSystemKey = segmentsEnumerator.Current;
                                AuthorizationSystem authorizationSystem;
                                if (!this.authorizationSystems.TryGetValue(authorizationSystemKey, out authorizationSystem))
                                {
                                    return new ODataResponse(
                                        404,
                                        Enumerable.Empty<string>(),
                                        GenerateStream(new ODataError(
                                            "NotFound",
                                            $"No entity with key '{authorizationSystemKey}' found in the collection at '/{string.Join('/', path)}'.",
                                            null,
                                            null)));
                                }

                                path.Add(authorizationSystemKey);
                                if (segmentsEnumerator.MoveNext())
                                {
                                    segment = segmentsEnumerator.Current;
                                    path.Add(segment);
                                    if (string.Equals(segment, "id"))
                                    {
                                        return new ODataResponse(
                                        200,
                                        Enumerable.Empty<string>(),
                                        GeneratePrimitiveStream(authorizationSystem.id));
                                    }
                                    else if (string.Equals(segment, "authorizationSystemName"))
                                    {
                                        return new ODataResponse(
                                            200,
                                            Enumerable.Empty<string>(),
                                            GeneratePrimitiveStream(authorizationSystem.authorizationSystemName));
                                    }
                                    else if (string.Equals(segment, "authorizationSystemType"))
                                    {
                                        return new ODataResponse(
                                            200,
                                            Enumerable.Empty<string>(),
                                            GeneratePrimitiveStream(authorizationSystem.authorizationSystemType));
                                    }
                                    else if (string.Equals(segment, "associatedIdentities"))
                                    {
                                        if (segmentsEnumerator.MoveNext())
                                        {
                                            segment = segmentsEnumerator.Current;
                                            if (string.Equals(segment, "all"))
                                            {
                                                path.Add(segment);
                                                var authorizationSystemIdentitySelector = (AuthorizationSystemIdentity _) => new { _.id, _.displayName };
                                                if (segmentsEnumerator.MoveNext())
                                                {
                                                    var authorizationSystemIdentityKey = segmentsEnumerator.Current;
                                                    AuthorizationSystemIdentity authorizationSystemIdentity;
                                                    if (!authorizationSystem.associatedIdentities.all.TryGetValue(authorizationSystemIdentityKey, out authorizationSystemIdentity))
                                                    {
                                                        return new ODataResponse(
                                                            404,
                                                            Enumerable.Empty<string>(),
                                                            GenerateStream(new ODataError(
                                                                "NotFound",
                                                                $"No entity with key '{authorizationSystemIdentityKey}' found in the collection at '/{string.Join('/', path)}'.",
                                                                null,
                                                                null)));
                                                    }

                                                    path.Add(authorizationSystemIdentityKey);
                                                    if (segmentsEnumerator.MoveNext())
                                                    {
                                                        //// TODO http://localhost:8080/external/authorizationSystems/1/associatedIdentities/all/second/graph.awsUser/assumableRoles/third <- is this a 404?
                                                        segment = segmentsEnumerator.Current;
                                                        if (string.Equals("id", segment))
                                                        {
                                                            return new ODataResponse(
                                                                200,
                                                                Enumerable.Empty<string>(),
                                                                GeneratePrimitiveStream(authorizationSystemIdentity.id));
                                                        }
                                                        else if (string.Equals("displayName", segment))
                                                        {
                                                            return new ODataResponse(
                                                                200,
                                                                Enumerable.Empty<string>(),
                                                                GeneratePrimitiveStream(authorizationSystemIdentity.displayName));
                                                        }
                                                        else if (string.Equals("graph.awsUser", segment) && authorizationSystemIdentity is AwsUser awsUser)
                                                        {
                                                            var awsUserSelector = authorizationSystemIdentitySelector;
                                                            if (segmentsEnumerator.MoveNext())
                                                            {
                                                                segment = segmentsEnumerator.Current;
                                                                path.Add(segment);
                                                                if (string.Equals("assumableRoles", segment))
                                                                {
                                                                    var awsRoleSelector = (AuthorizationSystemIdentity _) => new { _.id, _.displayName };
                                                                    if (segmentsEnumerator.MoveNext())
                                                                    {
                                                                        var assumableRoleKey = segmentsEnumerator.Current;
                                                                        AwsRole assumableRole;
                                                                        if (!awsUser.assumableRoles.TryGetValue(assumableRoleKey, out assumableRole))
                                                                        {
                                                                            return new ODataResponse(
                                                                                404,
                                                                                Enumerable.Empty<string>(),
                                                                                GenerateStream(new ODataError(
                                                                                    "NotFound",
                                                                                    $"No entity with key '{assumableRoleKey}' found in the collection at '/{string.Join('/', path)}'.",
                                                                                    null,
                                                                                    null)));
                                                                        }

                                                                        if (segmentsEnumerator.MoveNext())
                                                                        {
                                                                            //// TODO we are now recursing on authorizationsystemidentity
                                                                        }
                                                                        else
                                                                        {
                                                                            return new ODataResponse(
                                                                                200,
                                                                                Enumerable.Empty<string>(),
                                                                                GenerateStream(awsRoleSelector(assumableRole)));
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        return new ODataResponse(
                                                                            200,
                                                                            Enumerable.Empty<string>(),
                                                                            GenerateCollectionStream(awsUser.assumableRoles.Values.Select(awsRoleSelector)));
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    //// TODO we are now recursing on authorizationsystemidentity
                                                                    return new ODataResponse(
                                                                        501,
                                                                        Enumerable.Empty<string>(),
                                                                        GenerateStream(new ODataError(
                                                                            "NotImplemented",
                                                                            $"TODO this functionality has nopt been implemented in peachy",
                                                                            null,
                                                                            null)));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                return new ODataResponse(
                                                                    200,
                                                                    Enumerable.Empty<string>(),
                                                                    GenerateStream(awsUserSelector(awsUser)));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var entityTypeName = "microsoft.graph.authorizationSystemIdentity";
                                                            return new ODataResponse(
                                                                404, //// TODO 404 or 400?
                                                                Enumerable.Empty<string>(),
                                                                GenerateStream(new ODataError(
                                                                    "NotFound", //// TODO NotFound or BadRequest?
                                                                    $"The path '/{string.Join('/', path)}' refers to an instance of the entity type with name '{entityTypeName}'. There is no property with name '{segment}' defined on '{entityTypeName}'.",
                                                                    null,
                                                                    null)));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return new ODataResponse(
                                                            200,
                                                            Enumerable.Empty<string>(),
                                                            GenerateStream(authorizationSystemIdentitySelector(authorizationSystemIdentity)));
                                                    }
                                                }
                                                else
                                                {
                                                    return new ODataResponse(
                                                        200,
                                                        Enumerable.Empty<string>(),
                                                        GenerateCollectionStream(authorizationSystem.associatedIdentities.all.Values.Select(authorizationSystemIdentitySelector)));
                                                }
                                            }
                                            else
                                            {
                                                var complexTypeName = "microsoft.graph.associatedIdentities";
                                                return new ODataResponse(
                                                    404, //// TODO 404 or 400?
                                                    Enumerable.Empty<string>(),
                                                    GenerateStream(new ODataError(
                                                        "NotFound", //// TODO NotFound or BadRequest?
                                                        $"The path '/{string.Join('/', path)}' refers to an instance of the complex type with name '{complexTypeName}'. There is no property with name '{segment}' defined on '{complexTypeName}'.",
                                                        null,
                                                        null)));
                                            }
                                        }
                                        else
                                        {
                                            return new ODataResponse(
                                                200,
                                                Enumerable.Empty<string>(),
                                                GenerateStream(new
                                                {
                                                }));
                                        }
                                    }
                                    else
                                    {
                                        var entityTypeName = "microsoft.graph.authorizationSystem";
                                        return new ODataResponse(
                                            404, //// TODO 404 or 400?
                                            Enumerable.Empty<string>(),
                                            GenerateStream(new ODataError(
                                                "NotFound", //// TODO NotFound or BadRequest?
                                                $"The path '/{string.Join('/', path)}' refers to an entity of type '{entityTypeName}'. There is no property with name '{segment}' defined on '{entityTypeName}'.",
                                                null,
                                                null)));
                                    }
                                }
                                else
                                {
                                    return new ODataResponse(
                                        200,
                                        Enumerable.Empty<string>(),
                                        GenerateStream(authorizationSystemSelector(authorizationSystem)));
                                }
                            }
                            else
                            {
                                return new ODataResponse(
                                    200,
                                    Enumerable.Empty<string>(),
                                    GenerateCollectionStream(this.authorizationSystems.Values.Select(authorizationSystemSelector)));
                            }
                        }
                        else
                        {
                            var entityTypeName = "microsoft.graph.externalConnectors.external";
                            return new ODataResponse(
                                404, //// TODO 404 or 400?
                                Enumerable.Empty<string>(),
                                GenerateStream(new ODataError(
                                    "NotFound", //// TODO NotFound or BadRequest?
                                    $"The path '/{string.Join('/', path)}' refers to an entity of type '{entityTypeName}'. There is no property with name '{segment}' defined on '{entityTypeName}'.",
                                    null,
                                    null)));
                        }
                    }
                    else
                    {
                        return new ODataResponse(
                            404, //// TODO 404 or 400?
                            Enumerable.Empty<string>(),
                            GenerateStream(new ODataError(
                                "NotFound", //// TODO NotFound or BadRequest?
                                $"There is no singleton or entity set defined with name '{segment}'.", 
                                null, 
                                null)));
                    }
                }

                return await EmptyODataService.Instance.GetAsync(request);
            }

            private static Stream GeneratePrimitiveStream<T>(T value)
            {
                return GenerateStream(new { value = value });
            }

            private static Stream GenerateCollectionStream<T>(IEnumerable<T> values)
            {
                return GenerateStream(new { value = values });
            }

            private static Stream GenerateStream<T>(T response)
            {
                var serialzied = System.Text.Json.JsonSerializer.Serialize(response);
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(serialzied));
                stream.Position = 0;
                return stream;
            }
        }
    }

    public static class Extensions
    {
        public static IReadOnlyDictionary<TKey, TValueResult> SelectValue<TKey, TValueSource, TValueResult>(this IReadOnlyDictionary<TKey, TValueSource> source, Func<TValueSource, TValueResult> selector)
        {
            return new SelectValueReadOnlyDictionary<TKey, TValueSource, TValueResult>(source, selector);
        }

        private sealed class SelectValueReadOnlyDictionary<TKey, TValueSource, TValueResult> : IReadOnlyDictionary<TKey, TValueResult>
        {
            private readonly IReadOnlyDictionary<TKey, TValueSource> source;

            private readonly Func<TValueSource, TValueResult> selector;

            public SelectValueReadOnlyDictionary(IReadOnlyDictionary<TKey, TValueSource> source, Func<TValueSource, TValueResult> selector)
            {
                this.source = source;
                this.selector = selector;
            }

            public TValueResult this[TKey key] => this.selector(this.source[key]);

            public IEnumerable<TKey> Keys => this.source.Keys;

            public IEnumerable<TValueResult> Values => this.source.Values.Select(this.selector);

            public int Count => this.source.Count;

            public bool ContainsKey(TKey key)
            {
                return this.source.ContainsKey(key);
            }

            public IEnumerator<KeyValuePair<TKey, TValueResult>> GetEnumerator()
            {
                return this.source.Select(kvp => KeyValuePair.Create(kvp.Key, this.selector(kvp.Value))).GetEnumerator();
            }

            public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValueResult value)
            {
                var result = this.source.TryGetValue(key, out var sourceValue);
                value = this.selector(sourceValue);
                return result;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}