namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geometryPointParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geometryPoint> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geometryPoint>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geometryPoint> Parse(IInput<char>? input)
            {
                var _geometryPrefix_1 = __GeneratedOdataV3.Parsers.Rules._geometryPrefixParser.Instance.Parse(input);
if (!_geometryPrefix_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geometryPoint)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_geometryPrefix_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geometryPoint)!, input);
}

var _fullPointLiteral_1 = __GeneratedOdataV3.Parsers.Rules._fullPointLiteralParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_fullPointLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geometryPoint)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_fullPointLiteral_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geometryPoint)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geometryPoint(_geometryPrefix_1.Parsed, _SQUOTE_1.Parsed, _fullPointLiteral_1.Parsed, _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
