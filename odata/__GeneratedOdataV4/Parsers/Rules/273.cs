namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _binaryParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._binary> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._binary>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._binary> Parse(IInput<char>? input)
            {
                var _ʺx62x69x6Ex61x72x79ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx62x69x6Ex61x72x79ʺParser.Instance.Parse(input);
if (!_ʺx62x69x6Ex61x72x79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._binary)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_ʺx62x69x6Ex61x72x79ʺ_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._binary)!, input);
}

var _binaryValue_1 = __GeneratedOdataV4.Parsers.Rules._binaryValueParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_binaryValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._binary)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_binaryValue_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._binary)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._binary(_ʺx62x69x6Ex61x72x79ʺ_1.Parsed, _SQUOTE_1.Parsed, _binaryValue_1.Parsed, _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
