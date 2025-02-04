namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _isolationParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._isolation> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._isolation>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._isolation> Parse(IInput<char>? input)
            {
                var _ʺx4Fx44x61x74x61x2Dʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx4Fx44x61x74x61x2DʺParser.Instance.Optional().Parse(input);
if (!_ʺx4Fx44x61x74x61x2Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._isolation)!, input);
}

var _ʺx49x73x6Fx6Cx61x74x69x6Fx6Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx49x73x6Fx6Cx61x74x69x6Fx6EʺParser.Instance.Parse(_ʺx4Fx44x61x74x61x2Dʺ_1.Remainder);
if (!_ʺx49x73x6Fx6Cx61x74x69x6Fx6Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._isolation)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_ʺx49x73x6Fx6Cx61x74x69x6Fx6Eʺ_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._isolation)!, input);
}

var _OWS_1 = __GeneratedOdataV3.Parsers.Rules._OWSParser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_OWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._isolation)!, input);
}

var _ʺx73x6Ex61x70x73x68x6Fx74ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺParser.Instance.Parse(_OWS_1.Remainder);
if (!_ʺx73x6Ex61x70x73x68x6Fx74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._isolation)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._isolation(_ʺx4Fx44x61x74x61x2Dʺ_1.Parsed.GetOrElse(null), _ʺx49x73x6Fx6Cx61x74x69x6Fx6Eʺ_1.Parsed, _ʺx3Aʺ_1.Parsed, _OWS_1.Parsed, _ʺx73x6Ex61x70x73x68x6Fx74ʺ_1.Parsed), _ʺx73x6Ex61x70x73x68x6Fx74ʺ_1.Remainder);
            }
        }
    }
    
}
