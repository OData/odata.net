namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx7BʺⳆʺx25x37x42ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx7BʺⳆʺx25x37x42ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx7BʺⳆʺx25x37x42ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx7BʺⳆʺx25x37x42ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx7BʺⳆʺx25x37x42ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx7BʺⳆʺx25x37x42ʺParser.Instance.Parse(input);
if (!_ʺx7BʺⳆʺx25x37x42ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx7BʺⳆʺx25x37x42ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx7BʺⳆʺx25x37x42ʺↃ(_ʺx7BʺⳆʺx25x37x42ʺ_1.Parsed), _ʺx7BʺⳆʺx25x37x42ʺ_1.Remainder);
            }
        }
    }
    
}
