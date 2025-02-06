namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡ↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ> Parse(IInput<char>? input)
            {
                var _ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡_1 = __GeneratedOdataV4.Parsers.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Parser.Instance.Parse(input);
if (!_ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ(_ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡_1.Parsed), _ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡_1.Remainder);
            }
        }
    }
    
}
