namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺParser.Instance.Parse(input);
if (!_ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃ(_ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ_1.Parsed), _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ_1.Remainder);
            }
        }
    }
    
}
