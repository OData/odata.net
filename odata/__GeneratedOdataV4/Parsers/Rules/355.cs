namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _allowEntityReferencesPreferenceParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._allowEntityReferencesPreference> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._allowEntityReferencesPreference>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._allowEntityReferencesPreference> Parse(IInput<char>? input)
            {
                var _ʺx6Fx64x61x74x61x2Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional().Parse(input);
if (!_ʺx6Fx64x61x74x61x2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._allowEntityReferencesPreference)!, input);
}

var _ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺParser.Instance.Parse(_ʺx6Fx64x61x74x61x2Eʺ_1.Remainder);
if (!_ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._allowEntityReferencesPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._allowEntityReferencesPreference(_ʺx6Fx64x61x74x61x2Eʺ_1.Parsed.GetOrElse(null), _ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ_1.Parsed), _ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ_1.Remainder);
            }
        }
    }
    
}
