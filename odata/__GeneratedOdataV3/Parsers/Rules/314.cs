namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullLineStringLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._fullLineStringLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._fullLineStringLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._fullLineStringLiteral> Parse(IInput<char>? input)
            {
                var _sridLiteral_1 = __GeneratedOdataV3.Parsers.Rules._sridLiteralParser.Instance.Parse(input);
if (!_sridLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fullLineStringLiteral)!, input);
}

var _lineStringLiteral_1 = __GeneratedOdataV3.Parsers.Rules._lineStringLiteralParser.Instance.Parse(_sridLiteral_1.Remainder);
if (!_lineStringLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fullLineStringLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._fullLineStringLiteral(_sridLiteral_1.Parsed,  _lineStringLiteral_1.Parsed), _lineStringLiteral_1.Remainder);
            }
        }
    }
    
}
