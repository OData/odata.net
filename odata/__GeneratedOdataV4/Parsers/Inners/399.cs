namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx2FʺⳆʺx25x32x46ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2FʺⳆʺx25x32x46ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2FʺⳆʺx25x32x46ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2FʺⳆʺx25x32x46ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx2FʺⳆʺx25x32x46ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺⳆʺx25x32x46ʺParser.Instance.Parse(input);
if (!_ʺx2FʺⳆʺx25x32x46ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2FʺⳆʺx25x32x46ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2FʺⳆʺx25x32x46ʺↃ(_ʺx2FʺⳆʺx25x32x46ʺ_1.Parsed), _ʺx2FʺⳆʺx25x32x46ʺ_1.Remainder);
            }
        }
    }
    
}
