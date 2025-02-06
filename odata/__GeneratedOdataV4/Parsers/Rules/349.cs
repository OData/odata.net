namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityidParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityid> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityid>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._entityid> Parse(IInput<char>? input)
            {
                var _ʺx4Fx44x61x74x61x2Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx4Fx44x61x74x61x2DʺParser.Instance.Optional().Parse(input);
if (!_ʺx4Fx44x61x74x61x2Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityid)!, input);
}

var _ʺx45x6Ex74x69x74x79x49x44ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx45x6Ex74x69x74x79x49x44ʺParser.Instance.Parse(_ʺx4Fx44x61x74x61x2Dʺ_1.Remainder);
if (!_ʺx45x6Ex74x69x74x79x49x44ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityid)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_ʺx45x6Ex74x69x74x79x49x44ʺ_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityid)!, input);
}

var _OWS_1 = __GeneratedOdataV4.Parsers.Rules._OWSParser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_OWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityid)!, input);
}

var _IRIⲻinⲻheader_1 = __GeneratedOdataV4.Parsers.Rules._IRIⲻinⲻheaderParser.Instance.Parse(_OWS_1.Remainder);
if (!_IRIⲻinⲻheader_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityid)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._entityid(_ʺx4Fx44x61x74x61x2Dʺ_1.Parsed.GetOrElse(null), _ʺx45x6Ex74x69x74x79x49x44ʺ_1.Parsed, _ʺx3Aʺ_1.Parsed, _OWS_1.Parsed, _IRIⲻinⲻheader_1.Parsed), _IRIⲻinⲻheader_1.Remainder);
            }
        }
    }
    
}
