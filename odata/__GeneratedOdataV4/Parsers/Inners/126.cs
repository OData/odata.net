namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Ↄ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Ↄ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Ↄ> Parse(IInput<char>? input)
            {
                var _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡_1 = __GeneratedOdataV4.Parsers.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser.Instance.Parse(input);
if (!_STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Ↄ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Ↄ(_STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡_1.Parsed), _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡_1.Remainder);
            }
        }
    }
    
}
