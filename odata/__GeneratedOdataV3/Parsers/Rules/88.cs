namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectItemParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectItem> Instance { get; } = (_STARParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectItem>(_allOperationsInSchemaParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectItem>(_꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃParser.Instance);
        
        public static class _STARParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectItem._STAR> Instance { get; } = from _STAR_1 in __GeneratedOdataV3.Parsers.Rules._STARParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectItem._STAR(_STAR_1);
        }
        
        public static class _allOperationsInSchemaParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectItem._allOperationsInSchema> Instance { get; } = from _allOperationsInSchema_1 in __GeneratedOdataV3.Parsers.Rules._allOperationsInSchemaParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectItem._allOperationsInSchema(_allOperationsInSchema_1);
        }
        
        public static class _꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectItem._꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectItem._꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectItem._꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ> Parse(IInput<char>? input)
                {
                    var _ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2FʺParser.Instance.Optional().Parse(input);
if (!_ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectItem._꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ)!, input);
}

var _ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃParser.Instance.Parse(_ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1.Remainder);
if (!_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectItem._꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectItem._꘡ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ꘡_ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ(_ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1.Parsed.GetOrElse(null),  _ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ_1.Parsed), _ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ_1.Remainder);
                }
            }
        }
    }
    
}
