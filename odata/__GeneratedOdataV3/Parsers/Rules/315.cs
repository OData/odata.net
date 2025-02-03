namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _lineStringLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._lineStringLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._lineStringLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._lineStringLiteral> Parse(IInput<char>? input)
            {
                var _ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺParser.Instance.Parse(input);
if (!_ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._lineStringLiteral)!, input);
}

var _lineStringData_1 = __GeneratedOdataV3.Parsers.Rules._lineStringDataParser.Instance.Parse(_ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ_1.Remainder);
if (!_lineStringData_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._lineStringLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._lineStringLiteral(_ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ_1.Parsed,  _lineStringData_1.Parsed), _lineStringData_1.Remainder);
            }
        }
    }
    
}
