namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _polygonData_ЖⲤCOMMA_polygonDataↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._polygonData_ЖⲤCOMMA_polygonDataↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._polygonData_ЖⲤCOMMA_polygonDataↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._polygonData_ЖⲤCOMMA_polygonDataↃ> Parse(IInput<char>? input)
            {
                var _polygonData_1 = __GeneratedOdataV3.Parsers.Rules._polygonDataParser.Instance.Parse(input);
if (!_polygonData_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._polygonData_ЖⲤCOMMA_polygonDataↃ)!, input);
}

var _ⲤCOMMA_polygonDataↃ_1 = Inners._ⲤCOMMA_polygonDataↃParser.Instance.Many().Parse(_polygonData_1.Remainder);
if (!_ⲤCOMMA_polygonDataↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._polygonData_ЖⲤCOMMA_polygonDataↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._polygonData_ЖⲤCOMMA_polygonDataↃ(_polygonData_1.Parsed,  _ⲤCOMMA_polygonDataↃ_1.Parsed), _ⲤCOMMA_polygonDataↃ_1.Remainder);
            }
        }
    }
    
}
