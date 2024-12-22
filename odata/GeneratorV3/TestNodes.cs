namespace GeneratorV3
{
    using System.Collections.Generic;

    public sealed class firstⲻrule
    {
        public firstⲻrule(GeneratorV3.secondⲻrule secondⲻrule_1)
        {
            this.secondⲻrule_1 = secondⲻrule_1;
        }

        public GeneratorV3.secondⲻrule secondⲻrule_1 { get; }
    }

    public abstract class secondⲻrule
    {
        private secondⲻrule()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(GeneratorV3.secondⲻrule node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(secondⲻrule.firstⲻrule node, TContext context);
            protected internal abstract TResult Accept(secondⲻrule.firstⲻrule_firstⲻrule node, TContext context);
            protected internal abstract TResult Accept(secondⲻrule.firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule node, TContext context);
            protected internal abstract TResult Accept(secondⲻrule.ⲤfirstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡ↃↃ node, TContext context);
            protected internal abstract TResult Accept(secondⲻrule.firstⲻrule_Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_꘡firstⲻrule_firstⲻrule꘡ node, TContext context);
        }

        /*public sealed class secondⲻrule
        {
        //// TODO this *could* happen in the ABNF
        }*/

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

        public sealed class firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule : secondⲻrule
        {
            public firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule(
                Inners.firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule_1)
            {
                this.firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule_1 = firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule_1;
            }

            public Inners.firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule_1 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class ⲤfirstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡ↃↃ : secondⲻrule
        {
            public ⲤfirstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡ↃↃ(Inners.ⲤfirstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡ↃↃ ⲤfirstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡ↃↃ_1)
            {
                this.ⲤfirstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡ↃↃ_1 = ⲤfirstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡ↃↃ_1;
            }

            public Inners.ⲤfirstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡ↃↃ ⲤfirstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡ↃↃ_1 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class firstⲻrule_Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_꘡firstⲻrule_firstⲻrule꘡ : secondⲻrule
        {
            public firstⲻrule_Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_꘡firstⲻrule_firstⲻrule꘡(
                GeneratorV3.firstⲻrule firstⲻrule_1,
                Inners.Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1,
                Inners.firstⲻrule_firstⲻrule? firstⲻrule_firstⲻrule_1)
            {
                this.firstⲻrule_1 = firstⲻrule_1;
                this.Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
                this.firstⲻrule_firstⲻrule_1 = firstⲻrule_firstⲻrule_1;
            }

            public GeneratorV3.firstⲻrule firstⲻrule_1 { get; }
            public Inners.Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }
            public Inners.firstⲻrule_firstⲻrule? firstⲻrule_firstⲻrule_1 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }

    public static class Inners
    {
        public sealed class ⲤfirstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡ↃↃ
        {
            public ⲤfirstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡ↃↃ(Inners.firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_1)
            {
                this.firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
            }

            public Inners.firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }
        }

        public abstract class firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ
        {
            private firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ()
            {
            }

            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(Inners.firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ node, TContext context)
                {
                    return node.Dispatch(this, context);
                }

                protected internal abstract TResult Accept(firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ.firstⲻrule node, TContext context);
                protected internal abstract TResult Accept(firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ.Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ node, TContext context);
            }

            public sealed class firstⲻrule : firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ
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

            public sealed class Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ : firstⲻruleⳆⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ
            {
                public Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ(Inners.Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1)
                {
                    this.Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
                }

                public Inners.Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }

        public sealed class firstⲻrule_꘡firstⲻrule꘡
        {
            public firstⲻrule_꘡firstⲻrule꘡(GeneratorV3.firstⲻrule firstⲻrule_1, GeneratorV3.firstⲻrule? firstⲻrule_2)
            {
                this.firstⲻrule_1 = firstⲻrule_1;
                this.firstⲻrule_2 = firstⲻrule_2;
            }

            public GeneratorV3.firstⲻrule firstⲻrule_1 { get; }
            public GeneratorV3.firstⲻrule? firstⲻrule_2 { get; }
        }

        public sealed class firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule
        {
            public firstⲻrule_ꓸжⲤfirstⲻrule_꘡firstⲻrule꘡Ↄ_ж꘡firstⲻrule_firstⲻrule꘡_ж1firstⲻrule_1жfirstⲻrule(
                GeneratorV3.firstⲻrule firstⲻrule_1,
                Inners.Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ? Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1,
                IEnumerable<Inners.firstⲻrule_firstⲻrule?> firstⲻrule_firstⲻrules_1, //// TODO be careful about the "s" for collections
                IEnumerable<GeneratorV3.firstⲻrule> firstⲻrules_1,
                IEnumerable<GeneratorV3.firstⲻrule> firstⲻrules_2)
            {
                this.firstⲻrule_1 = firstⲻrule_1;
                this.Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
                this.firstⲻrule_firstⲻrules_1 = firstⲻrule_firstⲻrules_1;
                this.firstⲻrules_1 = firstⲻrules_1;
                this.firstⲻrules_2 = firstⲻrules_2;
            }

            public GeneratorV3.firstⲻrule firstⲻrule_1 { get; }
            public Inners.Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ? Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }
            public IEnumerable<Inners.firstⲻrule_firstⲻrule?> firstⲻrule_firstⲻrules_1 { get; }
            public IEnumerable<GeneratorV3.firstⲻrule> firstⲻrules_1 { get; }
            public IEnumerable<GeneratorV3.firstⲻrule> firstⲻrules_2 { get; }
        }

        public sealed class Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ
        {
            public Ⲥfirstⲻrule_꘡firstⲻrule꘡Ↄ(Inners.firstⲻrule_꘡firstⲻrule꘡ firstⲻrule_꘡firstⲻrule꘡_1)
            {
                this.firstⲻrule_꘡firstⲻrule꘡_1 = firstⲻrule_꘡firstⲻrule꘡_1;
            }

            public Inners.firstⲻrule_꘡firstⲻrule꘡ firstⲻrule_꘡firstⲻrule꘡_1 { get; }
        }

        public sealed class firstⲻrule_firstⲻrule
        {
            public firstⲻrule_firstⲻrule(GeneratorV3.firstⲻrule firstⲻrule_1, GeneratorV3.firstⲻrule firstⲻrule_2)
            {
                this.firstⲻrule_1 = firstⲻrule_1;
                this.firstⲻrule_2 = firstⲻrule_2;
            }

            public GeneratorV3.firstⲻrule firstⲻrule_1 { get; }
            public GeneratorV3.firstⲻrule firstⲻrule_2 { get; }
        }
    }
}
