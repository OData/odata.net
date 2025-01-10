namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _cⲻnl_WSPParser
    {
        public static Parser<__Generated.CstNodes.Inners._cⲻnl_WSP> Instance { get; } = from _cⲻnl_1 in __Generated.Parsers.Rules._cⲻnlParser.Instance
from _WSP_1 in __Generated.Parsers.Rules._WSPParser.Instance
select new __Generated.CstNodes.Inners._cⲻnl_WSP(_cⲻnl_1, _WSP_1);
    }
    
}
