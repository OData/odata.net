namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx7DʺⳆʺx25x37x44ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx7DʺⳆʺx25x37x44ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx7DʺⳆʺx25x37x44ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx7DʺⳆʺx25x37x44ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx7DʺⳆʺx25x37x44ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx7DʺⳆʺx25x37x44ʺParser.Instance.Parse(input);
if (!_ʺx7DʺⳆʺx25x37x44ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx7DʺⳆʺx25x37x44ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx7DʺⳆʺx25x37x44ʺↃ(_ʺx7DʺⳆʺx25x37x44ʺ_1.Parsed), _ʺx7DʺⳆʺx25x37x44ʺ_1.Remainder);
            }
        }
    }
    
}
