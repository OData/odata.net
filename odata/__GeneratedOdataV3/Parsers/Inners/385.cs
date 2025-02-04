namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx5BʺⳆʺx25x35x42ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx5BʺⳆʺx25x35x42ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx5BʺⳆʺx25x35x42ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx5BʺⳆʺx25x35x42ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx5BʺⳆʺx25x35x42ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx5BʺⳆʺx25x35x42ʺParser.Instance.Parse(input);
if (!_ʺx5BʺⳆʺx25x35x42ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx5BʺⳆʺx25x35x42ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx5BʺⳆʺx25x35x42ʺↃ(_ʺx5BʺⳆʺx25x35x42ʺ_1.Parsed), _ʺx5BʺⳆʺx25x35x42ʺ_1.Remainder);
            }
        }
    }
    
}
