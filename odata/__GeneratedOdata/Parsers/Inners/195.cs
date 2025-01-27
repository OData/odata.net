namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx41x4Ex44ʺ_RWSParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx41x4Ex44ʺ_RWS> Instance { get; } = from _ʺx41x4Ex44ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx41x4Ex44ʺParser.Instance
from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx41x4Ex44ʺ_RWS(_ʺx41x4Ex44ʺ_1, _RWS_1);
    }
    
}
