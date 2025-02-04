namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._collectionLiteral> Parse(IInput<char>? input)
            {
                var _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺParser.Instance.Parse(input);
if (!_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionLiteral)!, input);
}

var _geoLiteral_1 = __GeneratedOdataV3.Parsers.Rules._geoLiteralParser.Instance.Parse(_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺ_1.Remainder);
if (!_geoLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionLiteral)!, input);
}

var _ⲤCOMMA_geoLiteralↃ_1 = Inners._ⲤCOMMA_geoLiteralↃParser.Instance.Many().Parse(_geoLiteral_1.Remainder);
if (!_ⲤCOMMA_geoLiteralↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionLiteral)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_ⲤCOMMA_geoLiteralↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._collectionLiteral(_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺ_1.Parsed, _geoLiteral_1.Parsed, _ⲤCOMMA_geoLiteralↃ_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
