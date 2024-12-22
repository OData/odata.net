namespace GeneratorV3
{
    class FⲺⲻˑᱹꓸжⲤↃᱼｰⳆ꘡Ʇoo
    {
    }

    public sealed class firstⲻrule
    {
        public firstⲻrule(GeneratorV3.secondⲻrule secondⲻrule_1)
        {
            this.secondⲻrule_1 = secondⲻrule_1;
        }

        public secondⲻrule secondⲻrule_1 { get; }
    }

    public abstract class secondⲻrule
    {
        private secondⲻrule()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(secondⲻrule node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(firstⲻrule node, TContext context);
        }

        public sealed class firstⲻrule : secondⲻrule
        {
            public firstⲻrule(GeneratorV3.firstⲻrule firstⲻrule_1)
            {
                this.firstⲻrule_1 = firstⲻrule_1;
            }

            public GeneratorV3.firstⲻrule firstⲻrule_1 { get; }
        }

        public sealed class firstⲻrule_firstⲻrule : secondⲻrule //// TODO i don't really like using _ for spaces *and* for the property name conflict resolution
        {
            public firstⲻrule_firstⲻrule(Inners.firstⲻrule_firstⲻrule firstⲻrule_firstⲻrule_1)
            {
                this.firstⲻrule_firstⲻrule_1 = firstⲻrule_firstⲻrule_1;
            }

            public Inners.firstⲻrule_firstⲻrule firstⲻrule_firstⲻrule_1 { get; }
        }
    }

    public static class Inners
    {
        public sealed class firstⲻrule_firstⲻrule
        {
            public firstⲻrule_firstⲻrule(GeneratorV3.firstⲻrule firstⲻrule_1, GeneratorV3.firstⲻrule firstⲻrule_2)
            {
                this.firstⲻrule_1 = firstⲻrule_1;
                this.firstⲻrule_2 = firstⲻrule_2;
            }

            public firstⲻrule firstⲻrule_1 { get; }
            public firstⲻrule firstⲻrule_2 { get; }
        }
    }
}
