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
            public TResult Visit(secondⲻrule node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(secondⲻrule.firstⲻrule node, TContext context);
            protected internal abstract TResult Accept(secondⲻrule.firstⲻrule_firstⲻrule node, TContext context);
            protected internal abstract TResult Accept(secondⲻrule.firstⲻrule_asteriskopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_asterisk꘡firstⲻrule_firstⲻrule꘡_asterisk1firstⲻrule_1asteriskfirstⲻrule node, TContext context);
            protected internal abstract TResult Accept(secondⲻrule.openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ node, TContext context);
            protected internal abstract TResult Accept(secondⲻrule.firstⲻrule_openfirstⲻrule_꘡firstⲻrule꘡Ↄ_꘡firstⲻrule_firstⲻrule꘡ node, TContext context);
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

        public sealed class firstⲻrule_firstⲻrule : secondⲻrule
        {
            public firstⲻrule_firstⲻrule(GeneratorV3.firstⲻrule firstⲻrule_1, GeneratorV3.firstⲻrule firstⲻrule_2)
            {
                this.firstⲻrule_1 = firstⲻrule_1;
                this.firstⲻrule_2 = firstⲻrule_2;
            }

            public GeneratorV3.firstⲻrule firstⲻrule_1 { get; }
            public GeneratorV3.firstⲻrule firstⲻrule_2 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class firstⲻrule_asteriskopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_asterisk꘡firstⲻrule_firstⲻrule꘡_asterisk1firstⲻrule_1asteriskfirstⲻrule : secondⲻrule
        {
            public firstⲻrule_asteriskopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_asterisk꘡firstⲻrule_firstⲻrule꘡_asterisk1firstⲻrule_1asteriskfirstⲻrule(GeneratorV3.firstⲻrule firstⲻrule_1, IEnumerable<Inners.openfirstⲻrule_꘡firstⲻrule꘡Ↄ?> openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1, IEnumerable<Inners.firstⲻrule_firstⲻrule?> firstⲻrule_firstⲻrule_1, IEnumerable<GeneratorV3.firstⲻrule> firstⲻrule_2, IEnumerable<GeneratorV3.firstⲻrule> firstⲻrule_3)
            {
                this.firstⲻrule_1 = firstⲻrule_1;
                this.openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
                this.firstⲻrule_firstⲻrule_1 = firstⲻrule_firstⲻrule_1;
                this.firstⲻrule_2 = firstⲻrule_2;
                this.firstⲻrule_3 = firstⲻrule_3;
            }

            public GeneratorV3.firstⲻrule firstⲻrule_1 { get; }
            public IEnumerable<Inners.openfirstⲻrule_꘡firstⲻrule꘡Ↄ?> openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }
            public IEnumerable<Inners.firstⲻrule_firstⲻrule?> firstⲻrule_firstⲻrule_1 { get; }
            public IEnumerable<GeneratorV3.firstⲻrule> firstⲻrule_2 { get; }
            public IEnumerable<GeneratorV3.firstⲻrule> firstⲻrule_3 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ : secondⲻrule
        {
            public openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ(Inners.openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ_1)
            {
                this.openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ_1 = openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ_1;
            }

            public Inners.openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ_1 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class firstⲻrule_openfirstⲻrule_꘡firstⲻrule꘡Ↄ_꘡firstⲻrule_firstⲻrule꘡ : secondⲻrule
        {
            public firstⲻrule_openfirstⲻrule_꘡firstⲻrule꘡Ↄ_꘡firstⲻrule_firstⲻrule꘡(GeneratorV3.firstⲻrule firstⲻrule_1, Inners.openfirstⲻrule_꘡firstⲻrule꘡Ↄ openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1, Inners.firstⲻrule_firstⲻrule? firstⲻrule_firstⲻrule_1)
            {
                this.firstⲻrule_1 = firstⲻrule_1;
                this.openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
                this.firstⲻrule_firstⲻrule_1 = firstⲻrule_firstⲻrule_1;
            }

            public GeneratorV3.firstⲻrule firstⲻrule_1 { get; }
            public Inners.openfirstⲻrule_꘡firstⲻrule꘡Ↄ openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }
            public Inners.firstⲻrule_firstⲻrule? firstⲻrule_firstⲻrule_1 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }

    public static class Inners
    {
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

        public sealed class openfirstⲻrule_꘡firstⲻrule꘡Ↄ
        {
            public openfirstⲻrule_꘡firstⲻrule꘡Ↄ(Inners.firstⲻrule_꘡firstⲻrule꘡ firstⲻrule_꘡firstⲻrule꘡_1)
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

        public abstract class firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ
        {
            private firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ()
            {
            }

            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(Inners.firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ node, TContext context)
                {
                    return node.Dispatch(this, context);
                }

                protected internal abstract TResult Accept(firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ.firstⲻrule node, TContext context);
                protected internal abstract TResult Accept(firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ.openfirstⲻrule_꘡firstⲻrule꘡Ↄ node, TContext context);
            }

            public sealed class firstⲻrule : firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ
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

            public sealed class openfirstⲻrule_꘡firstⲻrule꘡Ↄ : firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ
            {
                public openfirstⲻrule_꘡firstⲻrule꘡Ↄ(Inners.openfirstⲻrule_꘡firstⲻrule꘡Ↄ openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1)
                {
                    this.openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
                }

                public Inners.openfirstⲻrule_꘡firstⲻrule꘡Ↄ openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }

        public sealed class openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ
        {
            public openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ(Inners.firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_1)
            {
                this.firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
            }

            public Inners.firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }
        }

        public sealed class firstⲻrule_asteriskopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_asterisk꘡firstⲻrule_firstⲻrule꘡_asterisk1firstⲻrule_1asteriskfirstⲻrule
        {
            public firstⲻrule_asteriskopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_asterisk꘡firstⲻrule_firstⲻrule꘡_asterisk1firstⲻrule_1asteriskfirstⲻrule(GeneratorV3.firstⲻrule firstⲻrule_1, Inners.openfirstⲻrule_꘡firstⲻrule꘡Ↄ? openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1, IEnumerable<Inners.firstⲻrule_firstⲻrule?> firstⲻrule_firstⲻrules_1, IEnumerable<GeneratorV3.firstⲻrule> firstⲻrules_1, IEnumerable<GeneratorV3.firstⲻrule> firstⲻrules_2)
            {
                this.firstⲻrule_1 = firstⲻrule_1;
                this.openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
                this.firstⲻrule_firstⲻrules_1 = firstⲻrule_firstⲻrules_1;
                this.firstⲻrules_1 = firstⲻrules_1;
                this.firstⲻrules_2 = firstⲻrules_2;
            }

            public GeneratorV3.firstⲻrule firstⲻrule_1 { get; }
            public Inners.openfirstⲻrule_꘡firstⲻrule꘡Ↄ? openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }
            public IEnumerable<Inners.firstⲻrule_firstⲻrule?> firstⲻrule_firstⲻrules_1 { get; }
            public IEnumerable<GeneratorV3.firstⲻrule> firstⲻrules_1 { get; }
            public IEnumerable<GeneratorV3.firstⲻrule> firstⲻrules_2 { get; }
        }
    }

}
