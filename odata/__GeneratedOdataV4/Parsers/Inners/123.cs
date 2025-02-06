namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡> Parse(IInput<char>? input)
            {
                var _ⲤcomplexPropertyⳆcomplexColPropertyↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃParser.Instance.Parse(input);
if (!_ⲤcomplexPropertyⳆcomplexColPropertyↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡)!, input);
}

var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(_ⲤcomplexPropertyⳆcomplexColPropertyↃ_1.Remainder);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡)!, input);
}

var _qualifiedComplexTypeName_ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._qualifiedComplexTypeName_ʺx2FʺParser.Instance.Optional().Parse(_ʺx2Fʺ_1.Remainder);
if (!_qualifiedComplexTypeName_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡(_ⲤcomplexPropertyⳆcomplexColPropertyↃ_1.Parsed, _ʺx2Fʺ_1.Parsed, _qualifiedComplexTypeName_ʺx2Fʺ_1.Parsed.GetOrElse(null)), _qualifiedComplexTypeName_ʺx2Fʺ_1.Remainder);
            }
        }
    }
    
}
