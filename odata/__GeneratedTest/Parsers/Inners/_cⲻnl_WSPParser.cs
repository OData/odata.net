namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _cⲻnl_WSPParser
    {
        public static Parser<__GeneratedTest.CstNodes.Inners._cⲻnl_WSP> Instance { get; } = from _cⲻnl_1 in __GeneratedTest.Parsers.Rules._cⲻnlParser.Instance
from _WSP_1 in __GeneratedTest.Parsers.Rules._WSPParser.Instance
select new __GeneratedTest.CstNodes.Inners._cⲻnl_WSP(_cⲻnl_1, _WSP_1);
    }
    
}
