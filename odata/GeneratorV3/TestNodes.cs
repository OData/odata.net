namespace GeneratorV3
{
    using System.Collections.Generic;

    public sealed class _firstⲻrule
    {
        public _firstⲻrule(GeneratorV3._secondⲻrule _secondⲻrule_1)
        {
            this._secondⲻrule_1 = _secondⲻrule_1;
        }

        public GeneratorV3._secondⲻrule _secondⲻrule_1 { get; }
    }

    public abstract class _secondⲻrule
    {
        private _secondⲻrule()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_secondⲻrule node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(_secondⲻrule._firstⲻrule node, TContext context);
            protected internal abstract TResult Accept(_secondⲻrule._firstⲻrule_firstⲻrule node, TContext context);
            protected internal abstract TResult Accept(_secondⲻrule._firstⲻrule_asteriskopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_asterisk꘡firstⲻrule_firstⲻrule꘡_asteriskONEfirstⲻrule_ONEasteriskfirstⲻrule node, TContext context);
            protected internal abstract TResult Accept(_secondⲻrule._openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ node, TContext context);
            protected internal abstract TResult Accept(_secondⲻrule._firstⲻrule_openfirstⲻrule_꘡firstⲻrule꘡Ↄ_꘡firstⲻrule_firstⲻrule꘡ node, TContext context);
        }

        public sealed class _firstⲻrule : _secondⲻrule
        {
            public _firstⲻrule(GeneratorV3._firstⲻrule _firstⲻrule_1)
            {
                this._firstⲻrule_1 = _firstⲻrule_1;
            }

            public GeneratorV3._firstⲻrule _firstⲻrule_1 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class _firstⲻrule_firstⲻrule : _secondⲻrule
        {
            public _firstⲻrule_firstⲻrule(GeneratorV3._firstⲻrule _firstⲻrule_1, GeneratorV3._firstⲻrule _firstⲻrule_2)
            {
                this._firstⲻrule_1 = _firstⲻrule_1;
                this._firstⲻrule_2 = _firstⲻrule_2;
            }

            public GeneratorV3._firstⲻrule _firstⲻrule_1 { get; }
            public GeneratorV3._firstⲻrule _firstⲻrule_2 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class _firstⲻrule_asteriskopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_asterisk꘡firstⲻrule_firstⲻrule꘡_asteriskONEfirstⲻrule_ONEasteriskfirstⲻrule : _secondⲻrule
        {
            public _firstⲻrule_asteriskopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_asterisk꘡firstⲻrule_firstⲻrule꘡_asteriskONEfirstⲻrule_ONEasteriskfirstⲻrule(GeneratorV3._firstⲻrule _firstⲻrule_1, IEnumerable<Inners._openfirstⲻrule_꘡firstⲻrule꘡Ↄ> _openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1, IEnumerable<Inners._firstⲻrule_firstⲻrule?> _firstⲻrule_firstⲻrule_1, IEnumerable<GeneratorV3._firstⲻrule> _firstⲻrule_2, IEnumerable<GeneratorV3._firstⲻrule> _firstⲻrule_3)
            {
                this._firstⲻrule_1 = _firstⲻrule_1;
                this._openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = _openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
                this._firstⲻrule_firstⲻrule_1 = _firstⲻrule_firstⲻrule_1;
                this._firstⲻrule_2 = _firstⲻrule_2;
                this._firstⲻrule_3 = _firstⲻrule_3;
            }

            public GeneratorV3._firstⲻrule _firstⲻrule_1 { get; }
            public IEnumerable<Inners._openfirstⲻrule_꘡firstⲻrule꘡Ↄ> _openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }
            public IEnumerable<Inners._firstⲻrule_firstⲻrule?> _firstⲻrule_firstⲻrule_1 { get; }
            public IEnumerable<GeneratorV3._firstⲻrule> _firstⲻrule_2 { get; }
            public IEnumerable<GeneratorV3._firstⲻrule> _firstⲻrule_3 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class _openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ : _secondⲻrule
        {
            public _openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ(Inners._openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ _openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ_1)
            {
                this._openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ_1 = _openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ_1;
            }

            public Inners._openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ _openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ_1 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class _firstⲻrule_openfirstⲻrule_꘡firstⲻrule꘡Ↄ_꘡firstⲻrule_firstⲻrule꘡ : _secondⲻrule
        {
            public _firstⲻrule_openfirstⲻrule_꘡firstⲻrule꘡Ↄ_꘡firstⲻrule_firstⲻrule꘡(GeneratorV3._firstⲻrule _firstⲻrule_1, Inners._openfirstⲻrule_꘡firstⲻrule꘡Ↄ _openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1, Inners._firstⲻrule_firstⲻrule? _firstⲻrule_firstⲻrule_1)
            {
                this._firstⲻrule_1 = _firstⲻrule_1;
                this._openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = _openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
                this._firstⲻrule_firstⲻrule_1 = _firstⲻrule_firstⲻrule_1;
            }

            public GeneratorV3._firstⲻrule _firstⲻrule_1 { get; }
            public Inners._openfirstⲻrule_꘡firstⲻrule꘡Ↄ _openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }
            public Inners._firstⲻrule_firstⲻrule? _firstⲻrule_firstⲻrule_1 { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }

    public static class Inners
    {
        public sealed class _firstⲻrule_꘡firstⲻrule꘡
        {
            public _firstⲻrule_꘡firstⲻrule꘡(GeneratorV3._firstⲻrule _firstⲻrule_1, GeneratorV3._firstⲻrule? _firstⲻrule_2)
            {
                this._firstⲻrule_1 = _firstⲻrule_1;
                this._firstⲻrule_2 = _firstⲻrule_2;
            }

            public GeneratorV3._firstⲻrule _firstⲻrule_1 { get; }
            public GeneratorV3._firstⲻrule? _firstⲻrule_2 { get; }
        }

        public sealed class _openfirstⲻrule_꘡firstⲻrule꘡Ↄ
        {
            public _openfirstⲻrule_꘡firstⲻrule꘡Ↄ(Inners._firstⲻrule_꘡firstⲻrule꘡ _firstⲻrule_꘡firstⲻrule꘡_1)
            {
                this._firstⲻrule_꘡firstⲻrule꘡_1 = _firstⲻrule_꘡firstⲻrule꘡_1;
            }

            public Inners._firstⲻrule_꘡firstⲻrule꘡ _firstⲻrule_꘡firstⲻrule꘡_1 { get; }
        }

        public sealed class _firstⲻrule_firstⲻrule
        {
            public _firstⲻrule_firstⲻrule(GeneratorV3._firstⲻrule _firstⲻrule_1, GeneratorV3._firstⲻrule _firstⲻrule_2)
            {
                this._firstⲻrule_1 = _firstⲻrule_1;
                this._firstⲻrule_2 = _firstⲻrule_2;
            }

            public GeneratorV3._firstⲻrule _firstⲻrule_1 { get; }
            public GeneratorV3._firstⲻrule _firstⲻrule_2 { get; }
        }

        public abstract class _firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ
        {
            private _firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ()
            {
            }

            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ node, TContext context)
                {
                    return node.Dispatch(this, context);
                }

                protected internal abstract TResult Accept(_firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ._firstⲻrule node, TContext context);
                protected internal abstract TResult Accept(_firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ._openfirstⲻrule_꘡firstⲻrule꘡Ↄ node, TContext context);
            }

            public sealed class _firstⲻrule : _firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ
            {
                public _firstⲻrule(GeneratorV3._firstⲻrule _firstⲻrule_1)
                {
                    this._firstⲻrule_1 = _firstⲻrule_1;
                }

                public GeneratorV3._firstⲻrule _firstⲻrule_1 { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }

            public sealed class _openfirstⲻrule_꘡firstⲻrule꘡Ↄ : _firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ
            {
                public _openfirstⲻrule_꘡firstⲻrule꘡Ↄ(Inners._openfirstⲻrule_꘡firstⲻrule꘡Ↄ _openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1)
                {
                    this._openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = _openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
                }

                public Inners._openfirstⲻrule_꘡firstⲻrule꘡Ↄ _openfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }

        public sealed class _openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ
        {
            public _openfirstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡ↃↃ(Inners._firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ _firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_1)
            {
                this._firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 = _firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_1;
            }

            public Inners._firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ _firstⲻruleⳆopenfirstⲻrule_꘡firstⲻrule꘡Ↄ_1 { get; }
        }
    }

}
