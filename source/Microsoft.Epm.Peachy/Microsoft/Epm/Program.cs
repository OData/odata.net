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

            public async Task<Stream> GetAsync(string url, Stream request)
            {
                var odataUri = new Uri($"http://testing{url}", UriKind.Absolute);

                using (var segmentsEnumerator = odataUri.Segments.Select(segment => segment.Trim('/')).GetEnumerator())
                {
                    if (!segmentsEnumerator.MoveNext() || !segmentsEnumerator.MoveNext())
                    {
                        throw new InvalidOperationException("TODO no segments");
                    }

                    var segment = segmentsEnumerator.Current;
                    if (string.Equals(segment, "external"))
                    {
                        if (!segmentsEnumerator.MoveNext())
                        {
                            return GenerateStream(new
                            {
                            });
                        }

                        segment = segmentsEnumerator.Current;
                        if (string.Equals(segment, "authorizationSystems"))
                        {
                            var authorizationSystemSelector = (AuthorizationSystem _) => new { _.id, _.authorizationSystemName, _.authorizationSystemType };
                            if (segmentsEnumerator.MoveNext())
                            {
                                var authorizationSystemKey = segmentsEnumerator.Current;
                                AuthorizationSystem authorizationSystem;
                                if (!this.authorizationSystems.TryGetValue(authorizationSystemKey, out authorizationSystem))
                                {
                                    return GenerateStream(new { error = "this is a 404 because the entity with that id can't be found" });
                                }

                                if (segmentsEnumerator.MoveNext())
                                {
                                    segment = segmentsEnumerator.Current;
                                    if (string.Equals(segment, "id"))
                                    {
                                        return GeneratePrimitiveStream(authorizationSystem.id);
                                    }
                                    else if (string.Equals(segment, "authorizationSystemName"))
                                    {
                                        return GeneratePrimitiveStream(authorizationSystem.authorizationSystemName);
                                    }
                                    else if (string.Equals(segment, "authorizationSystemType"))
                                    {
                                        return GeneratePrimitiveStream(authorizationSystem.authorizationSystemType);
                                    }
                                    else if (string.Equals(segment, "associatedIdentities"))
                                    {
                                        if (segmentsEnumerator.MoveNext())
                                        {
                                            segment = segmentsEnumerator.Current;
                                            if (string.Equals(segment, "all"))
                                            {
                                                var authorizationSystemIdentitySelector = (AuthorizationSystemIdentity _) => new { _.id, _.displayName };
                                                if (segmentsEnumerator.MoveNext())
                                                {
                                                    var authorizationSystemIdentityKey = segmentsEnumerator.Current;
                                                    AuthorizationSystemIdentity authorizationSystemIdentity;
                                                    if (!authorizationSystem.associatedIdentities.all.TryGetValue(authorizationSystemIdentityKey, out authorizationSystemIdentity))
                                                    {
                                                        return GenerateStream(new { error = "this is a 404 because the entity with that id can't be found" });
                                                    }

                                                    if (segmentsEnumerator.MoveNext())
                                                    {
                                                        //// TODO http://localhost:8080/external/authorizationSystems/1/associatedIdentities/all/second/graph.awsUser/assumableRoles/third <- is this a 404?
                                                        segment = segmentsEnumerator.Current;
                                                        if (string.Equals("id", segment))
                                                        {
                                                            return GeneratePrimitiveStream(authorizationSystemIdentity.id);
                                                        }
                                                        else if (string.Equals("displayName", segment))
                                                        {
                                                            return GeneratePrimitiveStream(authorizationSystemIdentity.displayName);
                                                        }
                                                        else if (string.Equals("graph.awsUser", segment) && authorizationSystemIdentity is AwsUser awsUser)
                                                        {
                                                            var awsUserSelector = authorizationSystemIdentitySelector;
                                                            if (segmentsEnumerator.MoveNext())
                                                            {
                                                                segment = segmentsEnumerator.Current;
                                                                if (string.Equals("assumableRoles", segment))
                                                                {
                                                                    var awsRoleSelector = (AuthorizationSystemIdentity _) => new { _.id, _.displayName };
                                                                    if (segmentsEnumerator.MoveNext())
                                                                    {
                                                                        var assumableRoleKey = segmentsEnumerator.Current;
                                                                        AwsRole assumableRole;
                                                                        if (!awsUser.assumableRoles.TryGetValue(assumableRoleKey, out assumableRole))
                                                                        {
                                                                            return GenerateStream(new { error = "this is a 404 because the entity with that id can't be found" });
                                                                        }

                                                                        if (segmentsEnumerator.MoveNext())
                                                                        {
                                                                            //// TODO we are now recursing on authorizationsystemidentity
                                                                        }
                                                                        else
                                                                        {
                                                                            return GenerateStream(awsRoleSelector(assumableRole));
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        return GenerateCollectionStream(awsUser.assumableRoles.Values.Select(awsRoleSelector));
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    //// TODO 404
                                                                }
                                                            }
                                                            else
                                                            {
                                                                GenerateStream(awsUserSelector(awsUser));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //// TODO 404
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return GenerateStream(authorizationSystemIdentitySelector(authorizationSystemIdentity));
                                                    }
                                                }
                                                else
                                                {
                                                    return GenerateCollectionStream(authorizationSystem.associatedIdentities.all.Values.Select(authorizationSystemIdentitySelector));
                                                }
                                            }
                                            else
                                            {
                                                //// TODO 404
                                            }
                                        }
                                        else
                                        {
                                            return GenerateStream(new
                                            {
                                            });
                                        }
                                    }
                                    else
                                    {
                                        //// TODO 404
                                    }
                                }
                                else
                                {
                                    return GenerateStream(authorizationSystemSelector(authorizationSystem));
                                }
                            }
                            else
                            {
                                return GenerateCollectionStream(this.authorizationSystems.Values.Select(authorizationSystemSelector));
                            }
                        }
                    }
                }

                return await EmptyOData.Instance.GetAsync(url, request);
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