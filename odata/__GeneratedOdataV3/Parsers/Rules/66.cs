namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _expandPathParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandPath> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandPath>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._expandPath> Parse(IInput<char>? input)
            {
                var _ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2FʺParser.Instance.Optional().Parse(input);
if (!_ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._expandPath)!, input);
}

var _ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ_1 = Inners._ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡ↃParser.Instance.Many().Parse(_ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1.Remainder);
if (!_ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._expandPath)!, input);
}

var _ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Ↄ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ↃParser.Instance.Parse(_ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ_1.Remainder);
if (!_ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Ↄ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._expandPath)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._expandPath(_ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1.Parsed.GetOrElse(null), _ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ_1.Parsed,  _ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Ↄ_1.Parsed), _ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Ↄ_1.Remainder);
            }
        }
    }
    
}
