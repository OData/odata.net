namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _Жcⲻwsp_cⲻnlParser
    {
        public static Parser<__GeneratedTest.CstNodes.Inners._Жcⲻwsp_cⲻnl> Instance { get; } = from _cⲻwsp_1 in __GeneratedTest.Parsers.Rules._cⲻwspParser.Instance.Many()
from _cⲻnl_1 in __GeneratedTest.Parsers.Rules._cⲻnlParser.Instance
select new __GeneratedTest.CstNodes.Inners._Жcⲻwsp_cⲻnl(_cⲻwsp_1, _cⲻnl_1);
    }
    
}
