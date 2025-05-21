namespace NewStuff._Design._2_Clr.Sample
{
    public sealed class UsersClr : ICollectionClr<User, string>
    {
        public UsersClr()
        {
        }

        public IGetCollectionClr<User, string> Get()
        {
            throw new System.NotImplementedException();
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
