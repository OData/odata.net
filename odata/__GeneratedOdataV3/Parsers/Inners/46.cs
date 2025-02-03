namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_qualifiedComplexTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_qualifiedComplexTypeName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_qualifiedComplexTypeName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_qualifiedComplexTypeName> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_qualifiedComplexTypeName)!, input);
}

var _qualifiedComplexTypeName_1 = __GeneratedOdataV3.Parsers.Rules._qualifiedComplexTypeNameParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_qualifiedComplexTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_qualifiedComplexTypeName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_qualifiedComplexTypeName(_ʺx2Fʺ_1.Parsed,  _qualifiedComplexTypeName_1.Parsed), _qualifiedComplexTypeName_1.Remainder);
            }
        }
    }
    
}
