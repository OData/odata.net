using NewStuff._Design._1_Protocol;
using System;
using System.Linq.Expressions;

namespace NewStuff._Design._2_Clr.Sample
{
    public sealed class UsersClr : ICollectionClr<User, string>
    {
        public UsersClr()
        {
        }

        public IGetCollectionClr<User, string> Get()
        {
            return new GetCollectionClr();
        }

        private sealed class GetCollectionClr : IGetCollectionClr<User, string>
        {
            private readonly IMultiValuedProtocol multiValuedProtocol;

            public GetCollectionClr()
            {
            }

            public CollectionResponse<User, string> Evaluate()
            {
                var response = this.multiValuedProtocol.Get();

            }

            public IGetCollectionClr<User, string> Expand<TProperty>(Expression<Func<User, Property<TProperty>>> expander)
            {
                throw new NotImplementedException();
            }

            public IGetCollectionClr<User, string> Filter(Expression<Func<User, bool>> predicate)
            {
                throw new NotImplementedException();
            }

            public IGetCollectionClr<User, string> Select<TProperty>(Expression<Func<User, Property<TProperty>>> selector)
            {
                throw new NotImplementedException();
            }

            public IGetCollectionClr<User, string> Skip(int count)
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
