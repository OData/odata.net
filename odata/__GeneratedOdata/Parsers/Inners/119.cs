namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2FʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ> Instance { get; } = from _ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃParser.Instance
from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ(_ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_1, _ʺx2Fʺ_1);
    }
    
}
