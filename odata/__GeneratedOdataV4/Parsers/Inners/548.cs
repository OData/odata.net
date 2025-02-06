namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _pointData_ЖⲤCOMMA_pointDataↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._pointData_ЖⲤCOMMA_pointDataↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._pointData_ЖⲤCOMMA_pointDataↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._pointData_ЖⲤCOMMA_pointDataↃ> Parse(IInput<char>? input)
            {
                var _pointData_1 = __GeneratedOdataV4.Parsers.Rules._pointDataParser.Instance.Parse(input);
if (!_pointData_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._pointData_ЖⲤCOMMA_pointDataↃ)!, input);
}

var _ⲤCOMMA_pointDataↃ_1 = Inners._ⲤCOMMA_pointDataↃParser.Instance.Many().Parse(_pointData_1.Remainder);
if (!_ⲤCOMMA_pointDataↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._pointData_ЖⲤCOMMA_pointDataↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._pointData_ЖⲤCOMMA_pointDataↃ(_pointData_1.Parsed, _ⲤCOMMA_pointDataↃ_1.Parsed), _ⲤCOMMA_pointDataↃ_1.Remainder);
            }
        }
    }
    
}
