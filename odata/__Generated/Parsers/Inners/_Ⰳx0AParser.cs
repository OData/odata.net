namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _Ⰳx0AParser
    {
        public static Parser<__Generated.CstNodes.Inners._Ⰳx0A> Instance { get; } = from _0_1 in __Generated.Parsers.Inners._0Parser.Instance
from _A_1 in __Generated.Parsers.Inners._AParser.Instance
select new __Generated.CstNodes.Inners._Ⰳx0A(_0_1, _A_1);
    }
    
}
