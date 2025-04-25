namespace NewStuff.Odata.Headers.Inners
{
    public abstract class T
    {
        private T()
        {
        }

        public sealed class Upper : T
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : T
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
