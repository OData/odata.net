namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _qualifiedEntityTypeName_ʺx2FʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._qualifiedEntityTypeName_ʺx2Fʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._qualifiedEntityTypeName_ʺx2Fʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._qualifiedEntityTypeName_ʺx2Fʺ> Parse(IInput<char>? input)
            {
                var _qualifiedEntityTypeName_1 = __GeneratedOdataV3.Parsers.Rules._qualifiedEntityTypeNameParser.Instance.Parse(input);
if (!_qualifiedEntityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._qualifiedEntityTypeName_ʺx2Fʺ)!, input);
}

var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(_qualifiedEntityTypeName_1.Remainder);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._qualifiedEntityTypeName_ʺx2Fʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._qualifiedEntityTypeName_ʺx2Fʺ(_qualifiedEntityTypeName_1.Parsed,  _ʺx2Fʺ_1.Parsed), _ʺx2Fʺ_1.Remainder);
            }
        }
    }
    
}
