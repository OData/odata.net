namespace Microsoft.OData.EntityDataModel
{
    using System.Collections.Generic;
    using System.Linq;

    public sealed class EntityDataModel
    {
        public EdmElement RootElement
        {
            get
            {
                return new EdmElement()
                {
                    Name = "/",
                    SingletonElements = new[]
                    {
                        new EdmElement()
                        {
                            Name = "external",
                            CollectionElements = new[]
                            {
                                new EdmElement()
                                {
                                    Name = "authorizationSystems",
                                    SingletonElements = new[]
                                    {
                                        new EdmElement()
                                        {
                                            Name = "id"
                                        },
                                        new EdmElement()
                                        {
                                            Name = "authorizationSystemName"
                                        },
                                        new EdmElement()
                                        {
                                            Name = "authorizationSystemType"
                                        },
                                        new EdmElement()
                                        {
                                            Name = "associatedIdentities",
                                            CollectionElements = new[]
                                            {
                                                new EdmElement()
                                                {
                                                    Name = "all",
                                                    SingletonElements = new[]
                                                    {
                                                        new EdmElement()
                                                        {
                                                            Name = "id"
                                                        },
                                                        new EdmElement()
                                                        {
                                                            Name = "displayName"
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

        public EdmElementLookup RootElementLookup
        {
            get
            {
                return ToLookup(this.RootElement);
            }
        }

        private static EdmElementLookup ToLookup(EdmElement element)
        {
            return new EdmElementLookup()
            {
                Name = element.Name,
                SingletonElements = element.SingletonElements.ToDictionary(element => element.Name, ToLookup),
                CollectionElements = element.CollectionElements.ToDictionary(element => element.Name, ToLookup),
            };
        }
    }

    public sealed class EdmElement
    {
        public string Name { get; set; }

        public IEnumerable<EdmElement> SingletonElements { get; set; } = Enumerable.Empty<EdmElement>();

        public IEnumerable<EdmElement> CollectionElements { get; set; } = Enumerable.Empty<EdmElement>();
    }

    public sealed class EdmElementLookup
    {
        public string Name { get; set; }

        public IReadOnlyDictionary<string, EdmElementLookup> SingletonElements { get; set; } = new Dictionary<string, EdmElementLookup>();

        public IReadOnlyDictionary<string, EdmElementLookup> CollectionElements { get; set; } = new Dictionary<string, EdmElementLookup>();
    }
}
