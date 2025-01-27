namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x34x30ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx25x34x30ʺ> Instance { get; } = from _x25_1 in __GeneratedOdata.Parsers.Inners._x25Parser.Instance
from _x34_1 in __GeneratedOdata.Parsers.Inners._x34Parser.Instance
from _x30_1 in __GeneratedOdata.Parsers.Inners._x30Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx25x34x30ʺ(_x25_1, _x34_1, _x30_1);
    }
    
}
