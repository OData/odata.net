namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _serviceRootParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._serviceRoot> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._serviceRoot>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._serviceRoot> Parse(IInput<char>? input)
            {
                var _Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._serviceRoot)!, input);
}

var _ʺx3Ax2Fx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Ax2Fx2FʺParser.Instance.Parse(_Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃ_1.Remainder);
if (!_ʺx3Ax2Fx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._serviceRoot)!, input);
}

var _host_1 = __GeneratedOdataV3.Parsers.Rules._hostParser.Instance.Parse(_ʺx3Ax2Fx2Fʺ_1.Remainder);
if (!_host_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._serviceRoot)!, input);
}

var _ʺx3Aʺ_port_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Aʺ_portParser.Instance.Optional().Parse(_host_1.Remainder);
if (!_ʺx3Aʺ_port_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._serviceRoot)!, input);
}

var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(_ʺx3Aʺ_port_1.Remainder);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._serviceRoot)!, input);
}

var _Ⲥsegmentⲻnz_ʺx2FʺↃ_1 = Inners._Ⲥsegmentⲻnz_ʺx2FʺↃParser.Instance.Many().Parse(_ʺx2Fʺ_1.Remainder);
if (!_Ⲥsegmentⲻnz_ʺx2FʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._serviceRoot)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._serviceRoot(_Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃ_1.Parsed, _ʺx3Ax2Fx2Fʺ_1.Parsed, _host_1.Parsed, _ʺx3Aʺ_port_1.Parsed.GetOrElse(null), _ʺx2Fʺ_1.Parsed, _Ⲥsegmentⲻnz_ʺx2FʺↃ_1.Parsed), _Ⲥsegmentⲻnz_ʺx2FʺↃ_1.Remainder);
            }
        }
    }
    
}
