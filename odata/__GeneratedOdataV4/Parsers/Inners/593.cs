namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺParser.Instance.Parse(input);
if (!_ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6CʺↃ(_ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ_1.Parsed), _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺⳆʺx6Dx69x6Ex69x6Dx61x6Cʺ_1.Remainder);
            }
        }
    }
    
}
