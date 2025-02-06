namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤentityIdOption_ʺx26ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤentityIdOption_ʺx26ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤentityIdOption_ʺx26ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤentityIdOption_ʺx26ʺↃ> Parse(IInput<char>? input)
            {
                var _entityIdOption_ʺx26ʺ_1 = __GeneratedOdataV4.Parsers.Inners._entityIdOption_ʺx26ʺParser.Instance.Parse(input);
if (!_entityIdOption_ʺx26ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤentityIdOption_ʺx26ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤentityIdOption_ʺx26ʺↃ(_entityIdOption_ʺx26ʺ_1.Parsed), _entityIdOption_ʺx26ʺ_1.Remainder);
            }
        }
    }
    
}
