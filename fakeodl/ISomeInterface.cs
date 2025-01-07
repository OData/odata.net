namespace fakeodl
{
    public interface ISomeInterface
    {
        public string Something { get; }

        public string AnotherThing()
        {
            return "the other thing";
        }
    }
}
