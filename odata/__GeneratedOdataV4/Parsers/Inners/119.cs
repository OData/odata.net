namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2FʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ> Parse(IInput<char>? input)
            {
                var _ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃParser.Instance.Parse(input);
if (!_ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ)!, input);
}

var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(_ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_1.Remainder);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ(_ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_1.Parsed, _ʺx2Fʺ_1.Parsed), _ʺx2Fʺ_1.Remainder);
            }
        }
    }
    
}
