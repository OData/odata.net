namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectListItemParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectListItem> Instance { get; } = (_STARParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._selectListItem>(_allOperationsInSchemaParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._selectListItem>(_꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser.Instance);
        
        public static class _STARParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectListItem._STAR> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectListItem._STAR>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._selectListItem._STAR> Parse(IInput<char>? input)
                {
                    var _STAR_1 = __GeneratedOdataV4.Parsers.Rules._STARParser.Instance.Parse(input);
if (!_STAR_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._selectListItem._STAR)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._selectListItem._STAR(_STAR_1.Parsed), _STAR_1.Remainder);
                }
            }
        }
        
        public static class _allOperationsInSchemaParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectListItem._allOperationsInSchema> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectListItem._allOperationsInSchema>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._selectListItem._allOperationsInSchema> Parse(IInput<char>? input)
                {
                    var _allOperationsInSchema_1 = __GeneratedOdataV4.Parsers.Rules._allOperationsInSchemaParser.Instance.Parse(input);
if (!_allOperationsInSchema_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._selectListItem._allOperationsInSchema)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._selectListItem._allOperationsInSchema(_allOperationsInSchema_1.Parsed), _allOperationsInSchema_1.Remainder);
                }
            }
        }
        
        public static class _꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ> Parse(IInput<char>? input)
                {
                    var _qualifiedEntityTypeName_ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._qualifiedEntityTypeName_ʺx2FʺParser.Instance.Optional().Parse(input);
if (!_qualifiedEntityTypeName_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ)!, input);
}

var _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser.Instance.Parse(_qualifiedEntityTypeName_ʺx2Fʺ_1.Remainder);
if (!_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._selectListItem._꘡qualifiedEntityTypeName_ʺx2Fʺ꘡_ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ(_qualifiedEntityTypeName_ʺx2Fʺ_1.Parsed.GetOrElse(null), _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1.Parsed), _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ_1.Remainder);
                }
            }
        }
    }
    
}
