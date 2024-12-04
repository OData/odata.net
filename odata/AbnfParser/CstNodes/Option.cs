namespace AbnfParser.CstNodes
{
    using AbnfParser.CstNodes.Core;
    using System.Collections.Generic;

    public sealed class Option
    {
        public Option(
            x5B openBracket, 
            IEnumerable<Cwsp> prefixCwsps, 
            Alternation alternation, 
            IEnumerable<Cwsp> suffixCwsps, 
            x5D closeBracket)
        {
            OpenBracket = openBracket;
            PrefixCwsps = prefixCwsps;
            Alternation = alternation;
            SuffixCwsps = suffixCwsps;
            CloseBracket = closeBracket;
        }

        public x5B OpenBracket { get; }
        public IEnumerable<Cwsp> PrefixCwsps { get; }
        public Alternation Alternation { get; }
        public IEnumerable<Cwsp> SuffixCwsps { get; }
        public x5D CloseBracket { get; }
    }
}
