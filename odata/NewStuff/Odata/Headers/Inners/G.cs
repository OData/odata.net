namespace NewStuff.Odata.Headers.Inners
{
    public abstract class G
    {
        private G()
        {
        }

        public sealed class Upper : G
        {
            private Upper()
            {
            }

            public static Upper Instance { get; } = new Upper();
        }

        public sealed class Lower : G
        {
            private Lower()
            {
            }

            public static Lower Instance { get; } = new Lower();
        }
    }
}
