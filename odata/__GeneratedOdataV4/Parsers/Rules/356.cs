namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _callbackPreferenceParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._callbackPreference> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._callbackPreference>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._callbackPreference> Parse(IInput<char>? input)
            {
                var _ʺx6Fx64x61x74x61x2Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional().Parse(input);
if (!_ʺx6Fx64x61x74x61x2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._callbackPreference)!, input);
}

var _ʺx63x61x6Cx6Cx62x61x63x6Bʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx63x61x6Cx6Cx62x61x63x6BʺParser.Instance.Parse(_ʺx6Fx64x61x74x61x2Eʺ_1.Remainder);
if (!_ʺx63x61x6Cx6Cx62x61x63x6Bʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._callbackPreference)!, input);
}

var _OWS_1 = __GeneratedOdataV4.Parsers.Rules._OWSParser.Instance.Parse(_ʺx63x61x6Cx6Cx62x61x63x6Bʺ_1.Remainder);
if (!_OWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._callbackPreference)!, input);
}

var _ʺx3Bʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3BʺParser.Instance.Parse(_OWS_1.Remainder);
if (!_ʺx3Bʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._callbackPreference)!, input);
}

var _OWS_2 = __GeneratedOdataV4.Parsers.Rules._OWSParser.Instance.Parse(_ʺx3Bʺ_1.Remainder);
if (!_OWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._callbackPreference)!, input);
}

var _ʺx75x72x6Cʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx75x72x6CʺParser.Instance.Parse(_OWS_2.Remainder);
if (!_ʺx75x72x6Cʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._callbackPreference)!, input);
}

var _EQⲻh_1 = __GeneratedOdataV4.Parsers.Rules._EQⲻhParser.Instance.Parse(_ʺx75x72x6Cʺ_1.Remainder);
if (!_EQⲻh_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._callbackPreference)!, input);
}

var _DQUOTE_1 = __GeneratedOdataV4.Parsers.Rules._DQUOTEParser.Instance.Parse(_EQⲻh_1.Remainder);
if (!_DQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._callbackPreference)!, input);
}

var _URI_1 = __GeneratedOdataV4.Parsers.Rules._URIParser.Instance.Parse(_DQUOTE_1.Remainder);
if (!_URI_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._callbackPreference)!, input);
}

var _DQUOTE_2 = __GeneratedOdataV4.Parsers.Rules._DQUOTEParser.Instance.Parse(_URI_1.Remainder);
if (!_DQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._callbackPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._callbackPreference(_ʺx6Fx64x61x74x61x2Eʺ_1.Parsed.GetOrElse(null), _ʺx63x61x6Cx6Cx62x61x63x6Bʺ_1.Parsed, _OWS_1.Parsed, _ʺx3Bʺ_1.Parsed, _OWS_2.Parsed, _ʺx75x72x6Cʺ_1.Parsed, _EQⲻh_1.Parsed, _DQUOTE_1.Parsed, _URI_1.Parsed, _DQUOTE_2.Parsed), _DQUOTE_2.Remainder);
            }
        }
    }
    
}
