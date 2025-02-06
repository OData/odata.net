namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _returnPreferenceParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._returnPreference> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._returnPreference>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._returnPreference> Parse(IInput<char>? input)
            {
                var _ʺx72x65x74x75x72x6Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx72x65x74x75x72x6EʺParser.Instance.Parse(input);
if (!_ʺx72x65x74x75x72x6Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._returnPreference)!, input);
}

var _EQⲻh_1 = __GeneratedOdataV4.Parsers.Rules._EQⲻhParser.Instance.Parse(_ʺx72x65x74x75x72x6Eʺ_1.Remainder);
if (!_EQⲻh_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._returnPreference)!, input);
}

var _Ⲥʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺↃ_1 = __GeneratedOdataV4.Parsers.Inners._Ⲥʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺↃParser.Instance.Parse(_EQⲻh_1.Remainder);
if (!_Ⲥʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._returnPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._returnPreference(_ʺx72x65x74x75x72x6Eʺ_1.Parsed, _EQⲻh_1.Parsed, _Ⲥʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺↃ_1.Parsed), _Ⲥʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺↃ_1.Remainder);
            }
        }
    }
    
}
