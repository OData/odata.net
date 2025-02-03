namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectItemParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._selectItem> Instance { get; } = (_STARParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._selectItem>(_allOperationsInSchemaParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._selectItem>(_꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃParser.Instance);
        
        public static class _STARParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._selectItem._STAR> Instance { get; } = from _STAR_1 in __GeneratedOdataV2.Parsers.Rules._STARParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._selectItem._STAR(_STAR_1);
        }
        
        public static class _allOperationsInSchemaParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._selectItem._allOperationsInSchema> Instance { get; } = from _allOperationsInSchema_1 in __GeneratedOdataV2.Parsers.Rules._allOperationsInSchemaParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._selectItem._allOperationsInSchema(_allOperationsInSchema_1);
        }
        
        public static class _꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._selectItem._꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ> Instance { get; } = from _ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2FʺParser.Instance.Optional()
from _ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._selectItem._꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ(_ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1.GetOrElse(null), _ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ_1);
        }
    }
    
}
