namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ> Parse(IInput<char>? input)
            {
                var _RWS_1 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(input);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ)!, input);
}

var _Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃParser.Instance.Parse(_RWS_1.Remainder);
if (!_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ(_RWS_1.Parsed,  _Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ_1.Parsed), _Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ_1.Remainder);
            }
        }
    }
    
}
