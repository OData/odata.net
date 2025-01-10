namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _Ⰳx0DParser
    {
        public static Parser<__Generated.CstNodes.Inners._Ⰳx0D> Instance { get; } = from _0_1 in __Generated.Parsers.Inners._0Parser.Instance
from _D_1 in __Generated.Parsers.Inners._DParser.Instance
select new __Generated.CstNodes.Inners._Ⰳx0D(_0_1, _D_1);
    }
    
}
