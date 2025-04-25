namespace NewStuff.Odata.Headers.Inners
{
    public abstract class C
    {
        private C()
        {
        }

        public sealed class Upper : C
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : C
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
