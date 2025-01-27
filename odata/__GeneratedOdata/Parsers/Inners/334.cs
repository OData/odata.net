namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx68x61x73ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx68x61x73ʺ> Instance { get; } = from _x68_1 in __GeneratedOdata.Parsers.Inners._x68Parser.Instance
from _x61_1 in __GeneratedOdata.Parsers.Inners._x61Parser.Instance
from _x73_1 in __GeneratedOdata.Parsers.Inners._x73Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx68x61x73ʺ(_x68_1, _x61_1, _x73_1);
    }
    
}
