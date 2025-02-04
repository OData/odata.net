namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _waitPreferenceParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._waitPreference> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._waitPreference>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._waitPreference> Parse(IInput<char>? input)
            {
                var _ʺx77x61x69x74ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx77x61x69x74ʺParser.Instance.Parse(input);
if (!_ʺx77x61x69x74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._waitPreference)!, input);
}

var _EQⲻh_1 = __GeneratedOdataV3.Parsers.Rules._EQⲻhParser.Instance.Parse(_ʺx77x61x69x74ʺ_1.Remainder);
if (!_EQⲻh_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._waitPreference)!, input);
}

var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, null).Parse(_EQⲻh_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._waitPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._waitPreference(_ʺx77x61x69x74ʺ_1.Parsed, _EQⲻh_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed)), _DIGIT_1.Remainder);
            }
        }
    }
    
}
