namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx32x35ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx32x35ʺ> Instance { get; } = from _x32_1 in __GeneratedOdata.Parsers.Inners._x32Parser.Instance
from _x35_1 in __GeneratedOdata.Parsers.Inners._x35Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx32x35ʺ(_x32_1, _x35_1);
    }
    
}
