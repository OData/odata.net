namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx64x65x73x63ʺParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx64x65x73x63ʺ> Instance { get; } = from _x64_1 in __GeneratedOdataV2.Parsers.Inners._x64Parser.Instance
from _x65_1 in __GeneratedOdataV2.Parsers.Inners._x65Parser.Instance
from _x73_1 in __GeneratedOdataV2.Parsers.Inners._x73Parser.Instance
from _x63_1 in __GeneratedOdataV2.Parsers.Inners._x63Parser.Instance
select __GeneratedOdataV2.CstNodes.Inners._ʺx64x65x73x63ʺ.Instance;
    }
    
}
