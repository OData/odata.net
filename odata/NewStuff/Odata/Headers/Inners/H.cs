namespace NewStuff.Odata.Headers.Inners
{
    public abstract class H
    {
        private H()
        {
        }

        public sealed class Upper : H
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : H
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
