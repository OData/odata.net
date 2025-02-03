namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectListItemParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._selectListItem> Instance { get; } = (_STARParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._selectListItem>(_allOperationsInSchemaParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._selectListItem>(_꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser.Instance);
        
        public static class _STARParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._selectListItem._STAR> Instance { get; } = from _STAR_1 in __GeneratedOdataV2.Parsers.Rules._STARParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._selectListItem._STAR(_STAR_1);
        }
        
        public static class _allOperationsInSchemaParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._selectListItem._allOperationsInSchema> Instance { get; } = from _allOperationsInSchema_1 in __GeneratedOdataV2.Parsers.Rules._allOperationsInSchemaParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._selectListItem._allOperationsInSchema(_allOperationsInSchema_1);
        }
        
        public static class _꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ> Instance { get; } = from _qualifiedEntityTypeName_ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._qualifiedEntityTypeName_ʺx2FʺParser.Instance.Optional()
from _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ(_qualifiedEntityTypeName_ʺx2Fʺ_1.GetOrElse(null), _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1);
        }
    }
    
}
