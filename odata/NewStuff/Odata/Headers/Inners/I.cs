namespace NewStuff.Odata.Headers.Inners
{
    public abstract class I
    {
        private I()
        {
        }

        public sealed class Upper : I
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : I
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
