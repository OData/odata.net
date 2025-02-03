namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _maxpagesizePreferenceParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._maxpagesizePreference> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._maxpagesizePreference>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._maxpagesizePreference> Parse(IInput<char>? input)
            {
                var _ʺx6Fx64x61x74x61x2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional().Parse(input);
if (!_ʺx6Fx64x61x74x61x2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._maxpagesizePreference)!, input);
}

var _ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺParser.Instance.Parse(_ʺx6Fx64x61x74x61x2Eʺ_1.Remainder);
if (!_ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._maxpagesizePreference)!, input);
}

var _EQⲻh_1 = __GeneratedOdataV3.Parsers.Rules._EQⲻhParser.Instance.Parse(_ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ_1.Remainder);
if (!_EQⲻh_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._maxpagesizePreference)!, input);
}

var _oneToNine_1 = __GeneratedOdataV3.Parsers.Rules._oneToNineParser.Instance.Parse(_EQⲻh_1.Remainder);
if (!_oneToNine_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._maxpagesizePreference)!, input);
}

var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Many().Parse(_oneToNine_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._maxpagesizePreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._maxpagesizePreference(_ʺx6Fx64x61x74x61x2Eʺ_1.Parsed.GetOrElse(null), _ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ_1.Parsed, _EQⲻh_1.Parsed, _oneToNine_1.Parsed,  _DIGIT_1.Parsed), _DIGIT_1.Remainder);
            }
        }
    }
    
}
