namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _contextParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._context> Instance { get; } = from _ʺx23ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx23ʺParser.Instance
from _contextFragment_1 in __GeneratedOdataV2.Parsers.Rules._contextFragmentParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._context(_ʺx23ʺ_1, _contextFragment_1);
    }
    
}
