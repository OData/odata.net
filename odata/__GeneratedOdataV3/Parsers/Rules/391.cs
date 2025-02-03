namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _IPvFutureParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPvFuture> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPvFuture>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._IPvFuture> Parse(IInput<char>? input)
            {
                var _ʺx76ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx76ʺParser.Instance.Parse(input);
if (!_ʺx76ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPvFuture)!, input);
}

var _HEXDIG_1 = __GeneratedOdataV3.Parsers.Rules._HEXDIGParser.Instance.Repeat(1, null).Parse(_ʺx76ʺ_1.Remainder);
if (!_HEXDIG_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPvFuture)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_HEXDIG_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPvFuture)!, input);
}

var _ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃParser.Instance.Repeat(1, null).Parse(_ʺx2Eʺ_1.Remainder);
if (!_ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPvFuture)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._IPvFuture(_ʺx76ʺ_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._HEXDIG>(_HEXDIG_1.Parsed), _ʺx2Eʺ_1.Parsed,  new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Inners._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ>(_ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1.Parsed)), _ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1.Remainder);
            }
        }
    }
    
}
