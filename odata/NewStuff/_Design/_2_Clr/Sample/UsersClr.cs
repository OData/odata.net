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

        public UsersClr(IMultiValuedProtocol multiValuedProtocol)
        {
            this.multiValuedProtocol = multiValuedProtocol;
        }

        public IGetCollectionClr<User> Get()
        {
            return new GetCollectionClr(this.multiValuedProtocol.Get());
        }

        private sealed class GetCollectionClr : IGetCollectionClr<User>
        {
            private readonly IGetMultiValuedProtocol multiValuedProtocol;

            public GetCollectionClr(IGetMultiValuedProtocol multiValuedProtocol)
            {
                this.multiValuedProtocol = multiValuedProtocol;
            }

            public CollectionResponse<User> Evaluate()
            {
                var response = this.multiValuedProtocol.Evaluate();

                var users = response
                    .Value
                    .Select(singleValue => UsersClr.UserUtilities.Deserialize(singleValue))
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

            public IGetCollectionClr<User> Expand<TProperty>(Expression<Func<User, Property<TProperty>>> expander) //// TODO this could have `navigationproperty` even
            {
                return new GetCollectionClr(this.multiValuedProtocol.Expand(UserUtilities.AdaptExpand(expander)));
            }

            public IGetCollectionClr<User> Filter(Expression<Func<User, bool>> predicate)
            {
                return new GetCollectionClr(this.multiValuedProtocol.Filter(AdaptFilter(predicate)));
            }

            private static object AdaptFilter(Expression<Func<User, bool>> predicate)
            {
                return predicate;
            }

            public IGetCollectionClr<User> Select<TProperty>(Expression<Func<User, Property<TProperty>>> selector)
            {
                return new GetCollectionClr(this.multiValuedProtocol.Select(UsersClr.UserUtilities.AdaptSelect(selector)));
            }

            public IGetCollectionClr<User> Skip(int count)
            {
                return new GetCollectionClr(this.multiValuedProtocol.Skip(AdaptSkip(count)));
            }

            private static object AdaptSkip(int count)
            {
                return count;
            }
        }

        public IGetClr<User, string> Get(string key) //// TODO how are you handling alternate keys?
        {
            var keyPredicate = new KeyPredicate.SinglePart(new SinglePartKeyPredicate.Canonical(key));
            return new GetClr(this.multiValuedProtocol.Get(keyPredicate));
        }

        private sealed class GetClr : IGetClr<User, string>
        {
            private readonly IGetSingleValuedProtocol singleValuedProtocol;

            public GetClr(IGetSingleValuedProtocol singleValuedProtocol)
            {
                this.singleValuedProtocol = singleValuedProtocol;
            }

            public SingleValuedResponse<User> Evaluate()
            {
                var response = this.singleValuedProtocol.Evaluate();
                return new SingleValuedResponse<User>(UserUtilities.Deserialize(response.Value), null, null);
            }

            public IGetClr<User, string> Expand<TProperty>(Expression<Func<User, Property<TProperty>>> expander)
            {
                return new GetClr(this.singleValuedProtocol.Expand(UserUtilities.AdaptExpand(expander)));
            }

            public IGetClr<User, string> Select<TProperty>(Expression<Func<User, Property<TProperty>>> selector)
            {
                return new GetClr(this.singleValuedProtocol.Select(UserUtilities.AdaptSelect(selector)));
            }
        }

        public IPatchCollectionClr<User> Patch(string key, User entity)
        {
            return new PatchCollectionClr(
                this.multiValuedProtocol.Patch(
                    new KeyPredicate.SinglePart(new SinglePartKeyPredicate.Canonical(key)), 
                    UserUtilities.Serialize(entity)));
        }

        private sealed class PatchCollectionClr : IPatchCollectionClr<User>
        {
            private readonly IPatchSingleValuedProtocol singleValuedProtocol;

            public PatchCollectionClr(IPatchSingleValuedProtocol singleValuedProtocol)
            {
                this.singleValuedProtocol = singleValuedProtocol;
            }

            public User? Evaluate()
            {
                var response = this.singleValuedProtocol.Evaluate();

                if (response.Value == null)
                {
                    return null;
                }

                return UserUtilities.Deserialize(response.Value);
            }

            public IPatchCollectionClr<User> Expand<TProperty>(Expression<Func<User, Property<TProperty>>> expander)
            {
                return new PatchCollectionClr(this.singleValuedProtocol.Expand(UserUtilities.AdaptExpand(expander)));
            }

            public IPatchCollectionClr<User> Select<TProperty>(Expression<Func<User, Property<TProperty>>> selector)
            {
                return new PatchCollectionClr(this.singleValuedProtocol.Select(UserUtilities.AdaptSelect(selector)));
            }
        }

        public IPostCollectionClr<User> Post(User entity)
        {
            return new PostCollectionClr(
                this.multiValuedProtocol.Post(
                    UserUtilities.Serialize(entity)));
        }

        private sealed class PostCollectionClr : IPostCollectionClr<User>
        {
            private readonly IPostSingleValuedProtocol singleValuedProtocol;

            public PostCollectionClr(IPostSingleValuedProtocol singleValuedProtocol)
            {
                this.singleValuedProtocol = singleValuedProtocol;
            }

            public User? Evaluate()
            {
                var response = this.singleValuedProtocol.Evaluate();

                if (response.Value == null)
                {
                    return null;
                }

                return UserUtilities.Deserialize(response.Value);
            }

            public IPostCollectionClr<User> Expand<TProperty>(Expression<Func<User, Property<TProperty>>> expander)
            {
                return new PostCollectionClr(this.singleValuedProtocol.Expand(UserUtilities.AdaptExpand(expander)));
            }

            public IPostCollectionClr<User> Select<TProperty>(Expression<Func<User, Property<TProperty>>> selector)
            {
                return new PostCollectionClr(this.singleValuedProtocol.Select(UserUtilities.AdaptSelect(selector)));
            }
        }

        private static class UserUtilities
        {
            public static User Deserialize(SingleValue value)
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

            public static SingleValuedRequest Serialize(User user)
            {
                var primitiveProperties = new List<PrimitiveRequestProperty>(2);

                if (user.Id is NonNullableProperty<string>.Provided providedId)
                {
                    primitiveProperties.Add(new PrimitiveRequestProperty("id", providedId.Value));
                }

                if (user.DisplayName is NullableProperty<string>.Provided providedDisplayName)
                {
                    primitiveProperties.Add(new PrimitiveRequestProperty("displayName", providedDisplayName.Value));
                }
                else if (user.DisplayName is NullableProperty<string>.Null nullDisplayName)
                {
                    primitiveProperties.Add(new PrimitiveRequestProperty("displayName", null));
                }

                var multiValuedProperties = new List<MultiValuedRequestProperty>(1);

                if (user.DirectReports is NonNullableProperty<IEnumerable<User>>.Provided providedDirectReports)
                {
                    multiValuedProperties.Add(new MultiValuedRequestProperty("directReports", providedDirectReports.Value.Select(Serialize)));
                }

                return new SingleValuedRequest(
                    Enumerable.Empty<ComplexRequestProperty>(),
                    multiValuedProperties,
                    Enumerable.Empty<UntypedRequestProperty>(),
                    primitiveProperties,
                    Enumerable.Empty<DynamicRequestProperty>());
            }

            public static object AdaptExpand<TProperty>(Expression<Func<User, Property<TProperty>>> expander)
            {
                //// TODO you need to implement all of the expression adapters
                return expander;
            }

            public static object AdaptSelect<TProperty>(Expression<Func<User, Property<TProperty>>> selector)
            {
                return selector;
            }
        }
    }
}
