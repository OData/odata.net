namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _multiLineStringLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._multiLineStringLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._multiLineStringLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._multiLineStringLiteral> Parse(IInput<char>? input)
            {
                var _ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67x28ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67x28ʺParser.Instance.Parse(input);
if (!_ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67x28ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._multiLineStringLiteral)!, input);
}

var _lineStringData_ЖⲤCOMMA_lineStringDataↃ_1 = __GeneratedOdataV3.Parsers.Inners._lineStringData_ЖⲤCOMMA_lineStringDataↃParser.Instance.Optional().Parse(_ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67x28ʺ_1.Remainder);
if (!_lineStringData_ЖⲤCOMMA_lineStringDataↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._multiLineStringLiteral)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_lineStringData_ЖⲤCOMMA_lineStringDataↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._multiLineStringLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._multiLineStringLiteral(_ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67x28ʺ_1.Parsed, _lineStringData_ЖⲤCOMMA_lineStringDataↃ_1.Parsed.GetOrElse(null), _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
