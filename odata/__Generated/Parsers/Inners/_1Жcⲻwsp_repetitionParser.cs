namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _1Жcⲻwsp_repetitionParser
    {
        public static Parser<__Generated.CstNodes.Inners._1Жcⲻwsp_repetition> Instance { get; } = from _cⲻwsp_1 in __Generated.Parsers.Rules._cⲻwspParser.Instance.Many()
from _repetition_1 in __Generated.Parsers.Rules._repetitionParser.Instance
select new __Generated.CstNodes.Inners._1Жcⲻwsp_repetition(new __Generated.CstNodes.Inners.HelperRangedAtLeast1<__Generated.CstNodes.Rules._cⲻwsp>(_cⲻwsp_1), _repetition_1);
    }
    
}
