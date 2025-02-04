namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectListItemParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListItem> Instance { get; } = (_STARParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectListItem>(_allOperationsInSchemaParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectListItem>(_꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser.Instance);
        
        public static class _STARParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListItem._STAR> Instance { get; } = from _STAR_1 in __GeneratedOdataV3.Parsers.Rules._STARParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectListItem._STAR(_STAR_1);
        }
        
        public static class _allOperationsInSchemaParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListItem._allOperationsInSchema> Instance { get; } = from _allOperationsInSchema_1 in __GeneratedOdataV3.Parsers.Rules._allOperationsInSchemaParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectListItem._allOperationsInSchema(_allOperationsInSchema_1);
        }
        
        public static class _꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ> Parse(IInput<char>? input)
                {
                    var _qualifiedEntityTypeName_ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._qualifiedEntityTypeName_ʺx2FʺParser.Instance.Optional().Parse(input);
if (!_qualifiedEntityTypeName_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ)!, input);
}

var _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser.Instance.Parse(_qualifiedEntityTypeName_ʺx2Fʺ_1.Remainder);
if (!_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ(_qualifiedEntityTypeName_ʺx2Fʺ_1.Parsed.GetOrElse(null),  _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1.Parsed), _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1.Remainder);
                }
            }
        }
    }
    
}
