﻿namespace GeneratorV3.OldToNewConverters.Core
{
    public sealed class x31Converter
    {
        private x31Converter()
        {
        }

        public static x31Converter Instance { get; } = new x31Converter();

        public Abnf.Inners._x31 Convert(AbnfParser.CstNodes.Core.x31 x31)
        {
            return Abnf.Inners._x31.Instance;
        }
    }
}