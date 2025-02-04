namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _polygonLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._polygonLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._polygonLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._polygonLiteral> Parse(IInput<char>? input)
            {
                var _ʺx50x6Fx6Cx79x67x6Fx6Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx50x6Fx6Cx79x67x6Fx6EʺParser.Instance.Parse(input);
if (!_ʺx50x6Fx6Cx79x67x6Fx6Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._polygonLiteral)!, input);
}

var _polygonData_1 = __GeneratedOdataV3.Parsers.Rules._polygonDataParser.Instance.Parse(_ʺx50x6Fx6Cx79x67x6Fx6Eʺ_1.Remainder);
if (!_polygonData_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._polygonLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._polygonLiteral(_ʺx50x6Fx6Cx79x67x6Fx6Eʺ_1.Parsed, _polygonData_1.Parsed), _polygonData_1.Remainder);
            }
        }
    }
    
}
