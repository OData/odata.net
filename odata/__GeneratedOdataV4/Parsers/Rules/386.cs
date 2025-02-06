namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _authorityParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._authority> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._authority>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._authority> Parse(IInput<char>? input)
            {
                var _userinfo_ʺx40ʺ_1 = __GeneratedOdataV4.Parsers.Inners._userinfo_ʺx40ʺParser.Instance.Optional().Parse(input);
if (!_userinfo_ʺx40ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._authority)!, input);
}

var _host_1 = __GeneratedOdataV4.Parsers.Rules._hostParser.Instance.Parse(_userinfo_ʺx40ʺ_1.Remainder);
if (!_host_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._authority)!, input);
}

var _ʺx3Aʺ_port_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3Aʺ_portParser.Instance.Optional().Parse(_host_1.Remainder);
if (!_ʺx3Aʺ_port_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._authority)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._authority(_userinfo_ʺx40ʺ_1.Parsed.GetOrElse(null), _host_1.Parsed, _ʺx3Aʺ_port_1.Parsed.GetOrElse(null)), _ʺx3Aʺ_port_1.Remainder);
            }
        }
    }
    
}
