namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_qualifiedEntityTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName)!, input);
}

var _qualifiedEntityTypeName_1 = __GeneratedOdataV4.Parsers.Rules._qualifiedEntityTypeNameParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_qualifiedEntityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName(_ʺx2Fʺ_1.Parsed, _qualifiedEntityTypeName_1.Parsed), _qualifiedEntityTypeName_1.Remainder);
            }
        }
    }
    
}
