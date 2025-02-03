namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _contentⲻidParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._contentⲻid> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._contentⲻid>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._contentⲻid> Parse(IInput<char>? input)
            {
                var _ʺx43x6Fx6Ex74x65x6Ex74x2Dx49x44ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx43x6Fx6Ex74x65x6Ex74x2Dx49x44ʺParser.Instance.Parse(input);
if (!_ʺx43x6Fx6Ex74x65x6Ex74x2Dx49x44ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contentⲻid)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_ʺx43x6Fx6Ex74x65x6Ex74x2Dx49x44ʺ_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contentⲻid)!, input);
}

var _OWS_1 = __GeneratedOdataV3.Parsers.Rules._OWSParser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_OWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contentⲻid)!, input);
}

var _requestⲻid_1 = __GeneratedOdataV3.Parsers.Rules._requestⲻidParser.Instance.Parse(_OWS_1.Remainder);
if (!_requestⲻid_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contentⲻid)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._contentⲻid(_ʺx43x6Fx6Ex74x65x6Ex74x2Dx49x44ʺ_1.Parsed, _ʺx3Aʺ_1.Parsed, _OWS_1.Parsed,  _requestⲻid_1.Parsed), _requestⲻid_1.Remainder);
            }
        }
    }
    
}
