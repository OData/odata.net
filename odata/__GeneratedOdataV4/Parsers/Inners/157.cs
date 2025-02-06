namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺParser.Instance.Parse(input);
if (!_ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ(_ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ_1.Parsed), _ʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺ_1.Remainder);
            }
        }
    }
    
}
