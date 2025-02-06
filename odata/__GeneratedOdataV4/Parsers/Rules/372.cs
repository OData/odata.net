namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _BWSParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._BWS> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._BWS>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._BWS> Parse(IInput<char>? input)
            {
                var _ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ_1 = Inners._ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃParser.Instance.Many().Parse(input);
if (!_ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._BWS)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._BWS(_ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ_1.Parsed), _ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ_1.Remainder);
            }
        }
    }
    
}
