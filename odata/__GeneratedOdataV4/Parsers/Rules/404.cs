namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _queryParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._query> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._query>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._query> Parse(IInput<char>? input)
            {
                var _ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ_1 = Inners._ⲤpcharⳆʺx2FʺⳆʺx3FʺↃParser.Instance.Many().Parse(input);
if (!_ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._query)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._query(_ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ_1.Parsed), _ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ_1.Remainder);
            }
        }
    }
    
}
