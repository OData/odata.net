namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _sridLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._sridLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._sridLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._sridLiteral> Parse(IInput<char>? input)
            {
                var _ʺx53x52x49x44ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx53x52x49x44ʺParser.Instance.Parse(input);
if (!_ʺx53x52x49x44ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._sridLiteral)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_ʺx53x52x49x44ʺ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._sridLiteral)!, input);
}

var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, 5).Parse(_EQ_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._sridLiteral)!, input);
}

var _SEMI_1 = __GeneratedOdataV3.Parsers.Rules._SEMIParser.Instance.Parse(_DIGIT_1.Remainder);
if (!_SEMI_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._sridLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._sridLiteral(_ʺx53x52x49x44ʺ_1.Parsed, _EQ_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedFrom1To5<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed), _SEMI_1.Parsed), _SEMI_1.Remainder);
            }
        }
    }
    
}
