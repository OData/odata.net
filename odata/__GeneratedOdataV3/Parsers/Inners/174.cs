namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺParser.Instance.Parse(input);
if (!_ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ(_ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ_1.Parsed), _ʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺ_1.Remainder);
            }
        }
    }
    
}
