namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _trackChangesPreferenceParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._trackChangesPreference> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._trackChangesPreference>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._trackChangesPreference> Parse(IInput<char>? input)
            {
                var _ʺx6Fx64x61x74x61x2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional().Parse(input);
if (!_ʺx6Fx64x61x74x61x2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._trackChangesPreference)!, input);
}

var _ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺParser.Instance.Parse(_ʺx6Fx64x61x74x61x2Eʺ_1.Remainder);
if (!_ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._trackChangesPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._trackChangesPreference(_ʺx6Fx64x61x74x61x2Eʺ_1.Parsed.GetOrElse(null),  _ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ_1.Parsed), _ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ_1.Remainder);
            }
        }
    }
    
}
