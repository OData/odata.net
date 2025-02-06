namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _multiPolygonLiteralParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._multiPolygonLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._multiPolygonLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._multiPolygonLiteral> Parse(IInput<char>? input)
            {
                var _ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺParser.Instance.Parse(input);
if (!_ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._multiPolygonLiteral)!, input);
}

var _polygonData_ЖⲤCOMMA_polygonDataↃ_1 = __GeneratedOdataV4.Parsers.Inners._polygonData_ЖⲤCOMMA_polygonDataↃParser.Instance.Optional().Parse(_ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ_1.Remainder);
if (!_polygonData_ЖⲤCOMMA_polygonDataↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._multiPolygonLiteral)!, input);
}

var _CLOSE_1 = __GeneratedOdataV4.Parsers.Rules._CLOSEParser.Instance.Parse(_polygonData_ЖⲤCOMMA_polygonDataↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._multiPolygonLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._multiPolygonLiteral(_ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ_1.Parsed, _polygonData_ЖⲤCOMMA_polygonDataↃ_1.Parsed.GetOrElse(null), _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
