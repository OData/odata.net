namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _selectListItemParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._selectListItem> Instance { get; } = (_STARParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectListItem>(_allOperationsInSchemaParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectListItem>(_꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser.Instance);
        
        public static class _STARParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectListItem._STAR> Instance { get; } = from _STAR_1 in __GeneratedOdata.Parsers.Rules._STARParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectListItem._STAR(_STAR_1);
        }
        
        public static class _allOperationsInSchemaParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectListItem._allOperationsInSchema> Instance { get; } = from _allOperationsInSchema_1 in __GeneratedOdata.Parsers.Rules._allOperationsInSchemaParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectListItem._allOperationsInSchema(_allOperationsInSchema_1);
        }
        
        public static class _꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ> Instance { get; } = from _qualifiedEntityTypeName_ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._qualifiedEntityTypeName_ʺx2FʺParser.Instance.Optional()
from _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ(_qualifiedEntityTypeName_ʺx2Fʺ_1.GetOrElse(null), _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1);
        }
    }
    
}