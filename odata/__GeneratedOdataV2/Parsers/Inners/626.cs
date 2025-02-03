namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x32x42ʺParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx25x32x42ʺ> Instance { get; } = from _x25_1 in __GeneratedOdataV2.Parsers.Inners._x25Parser.Instance
from _x32_1 in __GeneratedOdataV2.Parsers.Inners._x32Parser.Instance
from _x42_1 in __GeneratedOdataV2.Parsers.Inners._x42Parser.Instance
select __GeneratedOdataV2.CstNodes.Inners._ʺx25x32x42ʺ.Instance;
    }
    
}
