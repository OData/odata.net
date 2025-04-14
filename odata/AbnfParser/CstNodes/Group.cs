namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    using AbnfParser.CstNodes.Core;

    public sealed class Group
    {
        public Group(
            x28 openParenthesis, 
            IEnumerable<Cwsp> prefixCwsps, 
            Alternation alternation, 
            IEnumerable<Cwsp> suffixCwsps, 
            x29 closeParenthesis)
        {
            OpenParenthesis = openParenthesis;
            PrefixCwsps = prefixCwsps;
            Alternation = alternation;
            SuffixCwsps = suffixCwsps;
            CloseParenthesis = closeParenthesis;
        }

        public x28 OpenParenthesis { get; }
        public IEnumerable<Cwsp> PrefixCwsps { get; }
        public Alternation Alternation { get; }
        public IEnumerable<Cwsp> SuffixCwsps { get; }
        public x29 CloseParenthesis { get; }
    }
}
