namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullPointLiteralParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._fullPointLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._fullPointLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._fullPointLiteral> Parse(IInput<char>? input)
            {
                var _sridLiteral_1 = __GeneratedOdataV4.Parsers.Rules._sridLiteralParser.Instance.Parse(input);
if (!_sridLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._fullPointLiteral)!, input);
}

var _pointLiteral_1 = __GeneratedOdataV4.Parsers.Rules._pointLiteralParser.Instance.Parse(_sridLiteral_1.Remainder);
if (!_pointLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._fullPointLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._fullPointLiteral(_sridLiteral_1.Parsed, _pointLiteral_1.Parsed), _pointLiteral_1.Remainder);
            }
        }
    }
    
}
