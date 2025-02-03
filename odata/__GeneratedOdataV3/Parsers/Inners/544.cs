namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _lineStringData_ЖⲤCOMMA_lineStringDataↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._lineStringData_ЖⲤCOMMA_lineStringDataↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._lineStringData_ЖⲤCOMMA_lineStringDataↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._lineStringData_ЖⲤCOMMA_lineStringDataↃ> Parse(IInput<char>? input)
            {
                var _lineStringData_1 = __GeneratedOdataV3.Parsers.Rules._lineStringDataParser.Instance.Parse(input);
if (!_lineStringData_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._lineStringData_ЖⲤCOMMA_lineStringDataↃ)!, input);
}

var _ⲤCOMMA_lineStringDataↃ_1 = Inners._ⲤCOMMA_lineStringDataↃParser.Instance.Many().Parse(_lineStringData_1.Remainder);
if (!_ⲤCOMMA_lineStringDataↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._lineStringData_ЖⲤCOMMA_lineStringDataↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._lineStringData_ЖⲤCOMMA_lineStringDataↃ(_lineStringData_1.Parsed,  _ⲤCOMMA_lineStringDataↃ_1.Parsed), _ⲤCOMMA_lineStringDataↃ_1.Remainder);
            }
        }
    }
    
}
