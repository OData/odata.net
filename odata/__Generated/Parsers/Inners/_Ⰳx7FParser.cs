namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _Ⰳx7FParser
    {
        public static Parser<__Generated.CstNodes.Inners._Ⰳx7F> Instance { get; } = from _7_1 in __Generated.Parsers.Inners._7Parser.Instance
from _F_1 in __Generated.Parsers.Inners._FParser.Instance
select new __Generated.CstNodes.Inners._Ⰳx7F(_7_1, _F_1);
    }
    
}
