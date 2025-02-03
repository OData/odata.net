namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _IPv4addressParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv4address> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv4address>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._IPv4address> Parse(IInput<char>? input)
            {
                var _decⲻoctet_1 = __GeneratedOdataV3.Parsers.Rules._decⲻoctetParser.Instance.Parse(input);
if (!_decⲻoctet_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv4address)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_decⲻoctet_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv4address)!, input);
}

var _decⲻoctet_2 = __GeneratedOdataV3.Parsers.Rules._decⲻoctetParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_decⲻoctet_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv4address)!, input);
}

var _ʺx2Eʺ_2 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_decⲻoctet_2.Remainder);
if (!_ʺx2Eʺ_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv4address)!, input);
}

var _decⲻoctet_3 = __GeneratedOdataV3.Parsers.Rules._decⲻoctetParser.Instance.Parse(_ʺx2Eʺ_2.Remainder);
if (!_decⲻoctet_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv4address)!, input);
}

var _ʺx2Eʺ_3 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_decⲻoctet_3.Remainder);
if (!_ʺx2Eʺ_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv4address)!, input);
}

var _decⲻoctet_4 = __GeneratedOdataV3.Parsers.Rules._decⲻoctetParser.Instance.Parse(_ʺx2Eʺ_3.Remainder);
if (!_decⲻoctet_4.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv4address)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._IPv4address(_decⲻoctet_1.Parsed, _ʺx2Eʺ_1.Parsed, _decⲻoctet_2.Parsed, _ʺx2Eʺ_2.Parsed, _decⲻoctet_3.Parsed, _ʺx2Eʺ_3.Parsed,  _decⲻoctet_4.Parsed), _decⲻoctet_4.Remainder);
            }
        }
    }
    
}
