namespace NewStuff.Odata.Headers.Inners
{
    public abstract class A
    {
        private A()
        {
        }

        public sealed class Upper : A
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : A
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
