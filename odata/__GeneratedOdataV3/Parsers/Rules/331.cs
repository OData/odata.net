namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _positionLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._positionLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._positionLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._positionLiteral> Parse(IInput<char>? input)
            {
                var _doubleValue_1 = __GeneratedOdataV3.Parsers.Rules._doubleValueParser.Instance.Parse(input);
if (!_doubleValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._positionLiteral)!, input);
}

var _SP_1 = __GeneratedOdataV3.Parsers.Rules._SPParser.Instance.Parse(_doubleValue_1.Remainder);
if (!_SP_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._positionLiteral)!, input);
}

var _doubleValue_2 = __GeneratedOdataV3.Parsers.Rules._doubleValueParser.Instance.Parse(_SP_1.Remainder);
if (!_doubleValue_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._positionLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._positionLiteral(_doubleValue_1.Parsed, _SP_1.Parsed, _doubleValue_2.Parsed), _doubleValue_2.Remainder);
            }
        }
    }
    
}
