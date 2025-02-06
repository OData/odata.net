namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _navigationParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._navigation> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._navigation>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._navigation> Parse(IInput<char>? input)
            {
                var _Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Ↄ_1 = Inners._Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡ↃParser.Instance.Many().Parse(input);
if (!_Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Ↄ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._navigation)!, input);
}

var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(_Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Ↄ_1.Remainder);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._navigation)!, input);
}

var _navigationProperty_1 = __GeneratedOdataV4.Parsers.Rules._navigationPropertyParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_navigationProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._navigation)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._navigation(_Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Ↄ_1.Parsed, _ʺx2Fʺ_1.Parsed, _navigationProperty_1.Parsed), _navigationProperty_1.Remainder);
            }
        }
    }
    
}
