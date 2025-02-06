namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _durationParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._duration> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._duration>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._duration> Parse(IInput<char>? input)
            {
                var _ʺx64x75x72x61x74x69x6Fx6Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx64x75x72x61x74x69x6Fx6EʺParser.Instance.Optional().Parse(input);
if (!_ʺx64x75x72x61x74x69x6Fx6Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._duration)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_ʺx64x75x72x61x74x69x6Fx6Eʺ_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._duration)!, input);
}

var _durationValue_1 = __GeneratedOdataV4.Parsers.Rules._durationValueParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_durationValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._duration)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_durationValue_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._duration)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._duration(_ʺx64x75x72x61x74x69x6Fx6Eʺ_1.Parsed.GetOrElse(null), _SQUOTE_1.Parsed, _durationValue_1.Parsed, _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
