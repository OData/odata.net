namespace Microsoft.OData.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IGetData
    {
        bool TryGet(IReadOnlyDictionary<string, string> key, out object @object);

        IEnumerable<object> GetAll();
    }

    public sealed class DictionaryDataStore : IGetData
    {
        private readonly IReadOnlyDictionary<string, object> data;

        private readonly Func<IReadOnlyDictionary<string, string>, string> keySelector;

        public DictionaryDataStore(IReadOnlyDictionary<string, object> data, Func<IReadOnlyDictionary<string, string>, string> keySelector)
        {
            this.data = data;
            this.keySelector = keySelector;
        }

        public bool TryGet(IReadOnlyDictionary<string, string> key, out object @object) //// TODO being a dictionary means we can't have two segments with the same name
        {
            var computedKey = this.keySelector(key);
            return this.data.TryGetValue(computedKey, out @object);
        }

        public IEnumerable<object> GetAll()
        {
            return this.data.Values;
        }
    }

    public sealed class EntityDataModel
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

        public EntityDataModel()
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

        public EdmElement.SingleValuedElement RootElement
        {
            get
            {
                var authorizationSystemsDataStore = new DictionaryDataStore(
                    this.authorizationSystems.ToDictionary(_ => _.Key, _ => (object)_.Value),
                    (key) =>
                    {
                        if (key.Count != 1)
                        {
                            throw new ArgumentException("required key parts are not found");
                        }

                        if (!key.TryGetValue("authorizationSystems", out var authorizationSystemId))
                        {
                            throw new ArgumentException("required key parts are not found");
                        }

                        return authorizationSystemId;
                    }
                    );
                var authorizationSystemIdentitiesDataStore = new DictionaryDataStore(
                    this.authorizationSystems.SelectMany(_ => _.Value.associatedIdentities.all.Select(identity => new { _.Key, identity.Value })).ToDictionary(_ => _.Key + _.Value.id, _ => (object)_.Value),
                    (key) =>
                    {
                        if (key.Count != 2)
                        {
                            throw new ArgumentException("required key parts are not found");
                        }

                        if (!key.TryGetValue("authorizationSystems", out var authorizationSystemId))
                        {
                            throw new ArgumentException("required key parts are not found");
                        }

                        if (!key.TryGetValue("all", out var authorizationSystemIdentityId))
                        {
                            throw new ArgumentException("required key parts are not found");
                        }

                        return authorizationSystemId + authorizationSystemIdentityId;
                    }
                    );
                var authorizationSystemIdentitySelector = (object _) => new { ((AuthorizationSystemIdentity)_).id, ((AuthorizationSystemIdentity)_).displayName };
                return new EdmElement.SingleValuedElement()
                {
                    Name = "/", //// TODO hav a root element derived type
                    ElementType = "/",
                    Selector = _ => _, //// TODO
                    SingleValuedElements = new[]
                    {
                        new EdmElement.SingleValuedElement()
                        {
                            Name = "external", //// TODO a singleton/single-valued contained navigation also needs to tell you where to get the data from
                            ElementType = "microsoft.graph.externalConnectors.external",
                            Selector = _ => _, //// TODO
                            MultiValuedElements = new[]
                            {
                                new EdmElement.MultiValuedElement()
                                {
                                    Name = "authorizationSystems",
                                    ElementType = "microsoft.graph.authorizationSystem",
                                    DataStore = authorizationSystemsDataStore,
                                    Selector = (_) => 
                                        new { ((AuthorizationSystem)_).id, ((AuthorizationSystem)_).authorizationSystemName, ((AuthorizationSystem)_).authorizationSystemType },
                                    SingleValuedElements = new[]
                                    {
                                        new EdmElement.SingleValuedElement()
                                        {
                                            Name = "id",
                                            ElementType = "Edm.String",
                                            Selector = _ => _, //// TODO
                                        },
                                        new EdmElement.SingleValuedElement()
                                        {
                                            Name = "authorizationSystemName",
                                            ElementType = "Edm.String",
                                            Selector = _ => _, //// TODO
                                        },
                                        new EdmElement.SingleValuedElement()
                                        {
                                            Name = "authorizationSystemType",
                                            ElementType = "Edm.String",
                                            Selector = _ => _, //// TODO
                                        },
                                        new EdmElement.SingleValuedElement()
                                        {
                                            Name = "associatedIdentities",
                                            ElementType = "microsoft.graph.associatedIdentities",
                                            Selector = _ => _, //// TODO
                                            MultiValuedElements = new[]
                                            {
                                                new EdmElement.MultiValuedElement()
                                                {
                                                    Name = "all",
                                                    ElementType = "microsoft.graph.authorizationSystemIdentity",
                                                    DataStore = authorizationSystemIdentitiesDataStore, //// TODO this needs to be able to come from a flat list, or from authsystemsdatastore; maybe...i'm not sure...
                                                    Selector = authorizationSystemIdentitySelector,
                                                    SingleValuedElements = new[]
                                                    {
                                                        new EdmElement.SingleValuedElement()
                                                        {
                                                            Name = "id",
                                                            ElementType = "Edm.String",
                                                            Selector = _ => _, //// TODO
                                                        },
                                                        new EdmElement.SingleValuedElement()
                                                        {
                                                            Name = "displayName",
                                                            ElementType = "Edm.String",
                                                            Selector = _ => _, //// TODO
                                                        },
                                                    },
                                                    MultiValuedElements = new[]
                                                    {
                                                        new EdmElement.MultiValuedElement()
                                                        {
                                                            Name = "assumableRoles",
                                                            ElementType = "microsoft.graph.awsRole",
                                                            Selector = authorizationSystemIdentitySelector,
                                                            DataStore = authorizationSystemIdentitiesDataStore,
                                                        },
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }

        public EdmElementLookup<EdmElement.SingleValuedElement> RootElementLookup
        {
            get
            {
                return ToLookup(this.RootElement);
            }
        }

        private static EdmElementLookup<EdmElement.SingleValuedElement> ToLookup(EdmElement.SingleValuedElement element)
        {
            return new EdmElementLookup<EdmElement.SingleValuedElement>()
            {
                Element = element,
                SingletonElements = element.SingleValuedElements.ToDictionary(element => element.Name, ToLookup),
                CollectionElements = element.MultiValuedElements.ToDictionary(element => element.Name, ToLookup),
            };
        }

        private static EdmElementLookup<EdmElement.MultiValuedElement> ToLookup(EdmElement.MultiValuedElement element)
        {
            return new EdmElementLookup<EdmElement.MultiValuedElement>()
            {
                Element = element,
                SingletonElements = element.SingleValuedElements.ToDictionary(element => element.Name, ToLookup),
                CollectionElements = element.MultiValuedElements.ToDictionary(element => element.Name, ToLookup),
            };
        }
    }

    public class EdmElement
    {
        private EdmElement()
        {
        }

        public string Name { get; set; }

        public string ElementType { get; set; }

        public Func<object, object> Selector { get; set; }

        public IEnumerable<SingleValuedElement> SingleValuedElements { get; set; } = Enumerable.Empty<SingleValuedElement>();

        public IEnumerable<MultiValuedElement> MultiValuedElements { get; set; } = Enumerable.Empty<MultiValuedElement>();

        public sealed class SingleValuedElement : EdmElement
        {
        }

        public sealed class MultiValuedElement : EdmElement
        {
            //// TODO public bool Containment { get; set; }
            public IGetData DataStore { get; set; }
        }
    }

    public interface IEdmElementLookup<T> where T : EdmElement
    {
        T Element { get; set; }
    }

    public class EdmElementLookupBase
    {
        public IReadOnlyDictionary<string, EdmElementLookup<EdmElement.SingleValuedElement>> SingletonElements { get; set; } = new Dictionary<string, EdmElementLookup<EdmElement.SingleValuedElement>>();

        public IReadOnlyDictionary<string, EdmElementLookup<EdmElement.MultiValuedElement>> CollectionElements { get; set; } = new Dictionary<string, EdmElementLookup<EdmElement.MultiValuedElement>>();
    }

    public class EdmElementLookup<T> : EdmElementLookupBase where T : EdmElement
    {
        public T Element { get; set; }
    }
}
