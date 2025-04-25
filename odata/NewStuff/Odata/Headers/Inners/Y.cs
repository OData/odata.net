namespace NewStuff.Odata.Headers.Inners
{
    public abstract class Y
    {
        private Y()
        {
        }

        public sealed class Upper : Y
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : Y
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
