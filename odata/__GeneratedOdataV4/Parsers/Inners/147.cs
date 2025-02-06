namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx61x73x63ʺⳆʺx64x65x73x63ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺParser.Instance.Parse(input);
if (!_ʺx61x73x63ʺⳆʺx64x65x73x63ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ(_ʺx61x73x63ʺⳆʺx64x65x73x63ʺ_1.Parsed), _ʺx61x73x63ʺⳆʺx64x65x73x63ʺ_1.Remainder);
            }
        }
    }
    
}
