namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _includeAnnotationsPreferenceParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._includeAnnotationsPreference> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._includeAnnotationsPreference>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._includeAnnotationsPreference> Parse(IInput<char>? input)
            {
                var _ʺx6Fx64x61x74x61x2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional().Parse(input);
if (!_ʺx6Fx64x61x74x61x2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._includeAnnotationsPreference)!, input);
}

var _ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺParser.Instance.Parse(_ʺx6Fx64x61x74x61x2Eʺ_1.Remainder);
if (!_ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._includeAnnotationsPreference)!, input);
}

var _EQⲻh_1 = __GeneratedOdataV3.Parsers.Rules._EQⲻhParser.Instance.Parse(_ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ_1.Remainder);
if (!_EQⲻh_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._includeAnnotationsPreference)!, input);
}

var _DQUOTE_1 = __GeneratedOdataV3.Parsers.Rules._DQUOTEParser.Instance.Parse(_EQⲻh_1.Remainder);
if (!_DQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._includeAnnotationsPreference)!, input);
}

var _annotationsList_1 = __GeneratedOdataV3.Parsers.Rules._annotationsListParser.Instance.Parse(_DQUOTE_1.Remainder);
if (!_annotationsList_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._includeAnnotationsPreference)!, input);
}

var _DQUOTE_2 = __GeneratedOdataV3.Parsers.Rules._DQUOTEParser.Instance.Parse(_annotationsList_1.Remainder);
if (!_DQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._includeAnnotationsPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._includeAnnotationsPreference(_ʺx6Fx64x61x74x61x2Eʺ_1.Parsed.GetOrElse(null), _ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ_1.Parsed, _EQⲻh_1.Parsed, _DQUOTE_1.Parsed, _annotationsList_1.Parsed, _DQUOTE_2.Parsed), _DQUOTE_2.Remainder);
            }
        }
    }
    
}
