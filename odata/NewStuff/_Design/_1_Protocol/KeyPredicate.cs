namespace NewStuff._Design._1_Protocol
{
    using System.Collections.Generic;

    public abstract class KeyPredicate
    {
        private KeyPredicate()
        {
        }

        public sealed class MultiPart : KeyPredicate
        {
            public MultiPart(IEnumerable<KeyArgument> keyArguments)
            {
                KeyArguments = keyArguments;
            }

            public IEnumerable<KeyArgument> KeyArguments { get; }
        }

        public sealed class SinglePart : KeyPredicate
        {
            public SinglePart(SinglePartKeyPredicate keyPredicate)
            {
                KeyPredicate = keyPredicate;
            }

            public SinglePartKeyPredicate KeyPredicate { get; }
        }
    }
}
