namespace NewStuff.Odata.Headers.Inners
{
    public abstract class U
    {
        private U()
        {
        }

        public sealed class Upper : U
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : U
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
