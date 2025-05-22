namespace NewStuff._Design._2_Clr.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using NewStuff._Design._1_Protocol;
    using NewStuff._Design._3_Context.Sample;

    public sealed class UsersClr : ICollectionClr<User, string>
    {
        private readonly IMultiValuedProtocol multiValuedProtocol;

        public UsersClr()
        {
        }

        public IGetCollectionClr<User> Get()
        {
            return new GetCollectionClr(this.multiValuedProtocol);
        }

        private sealed class GetCollectionClr : IGetCollectionClr<User>
        {
            private readonly IMultiValuedProtocol multiValuedProtocol;

            public GetCollectionClr(IMultiValuedProtocol multiValuedProtocol)
            {
                this.multiValuedProtocol = multiValuedProtocol;
            }

            public CollectionResponse<User> Evaluate()
            {
                var response = this.multiValuedProtocol.Get();

                var users = response
                    .Value
                    .Select(singleValue => Deserialize(singleValue))
                    .Select(user => 
                        new CollectionResponseEntity<User>(
                            new Entity<User>( //// TODO should the key be required to instantiate this? what does the standard say? is the key always returned?
                                user),
                            null,
                            null));

                return new CollectionResponse<User>(
                    users, 
                    response.NextLink, 
                    response.Count, 
                    null);
            }

            private static User Deserialize(SingleValue value)
            {
                //// TODO the strings in here are coming from "implicit knowledge" of the EDM model
                var primitiveProperties = value.PrimitiveProperties.ToDictionary(property => property.Name);

                NullableProperty<string> displayName;
                if (primitiveProperties.TryGetValue("displayName", out var providedDisplayName))
                {
                    if (providedDisplayName.Value == null)
                    {
                        displayName = NullableProperty.Null<string>();
                    }
                    else
                    {
                        displayName = NullableProperty.Provided(providedDisplayName.Value);
                    }
                }
                else
                {
                    displayName = NullableProperty.NotProvided<string>();
                }

                NonNullableProperty<string> id;
                if (primitiveProperties.TryGetValue("id", out var providedId))
                {
                    id = NonNullableProperty.Provided(providedId.Value)!; //// TODO at what point should we know that this isn't nullable? //// TODO maybe this is an indication that this is the layer where the edm model is starting to be used?
                }
                else
                {
                    id = NonNullableProperty.NotProvided<string>();
                }

                var multiValuedProperties = value.MultiValuedProperties.ToDictionary(property => property.Name);

                NonNullableProperty<IEnumerable<User>> directReports;
                if (multiValuedProperties.TryGetValue("directReports", out var providedDirectReports))
                {
                    directReports = NonNullableProperty.Provided(providedDirectReports.Values.Select(Deserialize));
                }
                else
                {
                    directReports = NonNullableProperty.NotProvided<IEnumerable<User>>();
                }

                return new User(displayName, directReports, id);
            }

            public IGetCollectionClr<User> Expand<TProperty>(Expression<Func<User, Property<TProperty>>> expander) //// TODO this could have `navigationproperty` even
            {
                throw new NotImplementedException();
            }

            public IGetCollectionClr<User> Filter(Expression<Func<User, bool>> predicate)
            {
                throw new NotImplementedException();
            }

            public IGetCollectionClr<User> Select<TProperty>(Expression<Func<User, Property<TProperty>>> selector)
            {
                throw new NotImplementedException();
            }

            public IGetCollectionClr<User> Skip(int count)
            {
                throw new NotImplementedException();
            }
        }

        public IGetClr<User, string> Get(string key)
        {
            throw new System.NotImplementedException();
        }

        public IPatchCollectionClr<User> Patch(string key, User entity)
        {
            throw new System.NotImplementedException();
        }

        public IPostCollectionClr<User> Post(User entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
