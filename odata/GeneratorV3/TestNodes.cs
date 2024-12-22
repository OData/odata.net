namespace GeneratorV3
{
    using System.Collections.Generic;

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
            protected internal abstract TResult Accept(firstⲻrule_firstⲻrule node, TContext context);
        }

        public sealed class firstⲻrule : secondⲻrule
        {
            public firstⲻrule(GeneratorV3.firstⲻrule firstⲻrule_1)
            {
                this.firstⲻrule_1 = firstⲻrule_1;
            }

            public GeneratorV3.firstⲻrule firstⲻrule_1 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class firstⲻrule_firstⲻrule : secondⲻrule //// TODO i don't really like using _ for spaces *and* for the property name conflict resolution
        {
            public firstⲻrule_firstⲻrule(Inners.firstⲻrule_firstⲻrule firstⲻrule_firstⲻrule_1)
            {
                this.firstⲻrule_firstⲻrule_1 = firstⲻrule_firstⲻrule_1;
            }

            public Inners.firstⲻrule_firstⲻrule firstⲻrule_firstⲻrule_1 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }

    public static class Inners
    {
        public sealed class firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule
        {
            public firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule(
                GeneratorV3.firstⲻrule firstⲻrule_1,
                Inners.Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ? Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1,
                IEnumerable<Inners.firstⲻrule_firstⲻrule?> firstⲻrule_firstⲻrules_1,
                IEnumerable<GeneratorV3.firstⲻrule> firstⲻrules_1,
                IEnumerable<GeneratorV3.firstⲻrule> firstⲻrules_2)
            {
                this.firstⲻrule_1 = firstⲻrule_1;
                this.Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
                this.firstⲻrule_firstⲻrules_1 = firstⲻrule_firstⲻrules_1;
                this.firstⲻrules_1 = firstⲻrules_1;
                this.firstⲻrules_2 = firstⲻrules_2;
            }

            public firstⲻrule firstⲻrule_1 { get; }
            public Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ? Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }
            public IEnumerable<firstⲻrule_firstⲻrule?> firstⲻrule_firstⲻrules_1 { get; }
            public IEnumerable<firstⲻrule> firstⲻrules_1 { get; }
            public IEnumerable<firstⲻrule> firstⲻrules_2 { get; }
        }

        public sealed class Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ
        {
        }

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
