namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤentitySetName_keyPredicateⳆsingletonEntityↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤentitySetName_keyPredicateⳆsingletonEntityↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤentitySetName_keyPredicateⳆsingletonEntityↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤentitySetName_keyPredicateⳆsingletonEntityↃ> Parse(IInput<char>? input)
            {
                var _entitySetName_keyPredicateⳆsingletonEntity_1 = __GeneratedOdataV4.Parsers.Inners._entitySetName_keyPredicateⳆsingletonEntityParser.Instance.Parse(input);
if (!_entitySetName_keyPredicateⳆsingletonEntity_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤentitySetName_keyPredicateⳆsingletonEntityↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤentitySetName_keyPredicateⳆsingletonEntityↃ(_entitySetName_keyPredicateⳆsingletonEntity_1.Parsed), _entitySetName_keyPredicateⳆsingletonEntity_1.Remainder);
            }
        }
    }
    
}
