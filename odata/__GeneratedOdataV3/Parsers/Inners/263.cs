namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _entitySetName_keyPredicateⳆsingletonEntityParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity> Instance { get; } = (_entitySetName_keyPredicateParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity>(_singletonEntityParser.Instance);
        
        public static class _entitySetName_keyPredicateParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._entitySetName_keyPredicate> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._entitySetName_keyPredicate>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._entitySetName_keyPredicate> Parse(IInput<char>? input)
                {
                    var _entitySetName_1 = __GeneratedOdataV3.Parsers.Rules._entitySetNameParser.Instance.Parse(input);
if (!_entitySetName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._entitySetName_keyPredicate)!, input);
}

var _keyPredicate_1 = __GeneratedOdataV3.Parsers.Rules._keyPredicateParser.Instance.Parse(_entitySetName_1.Remainder);
if (!_keyPredicate_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._entitySetName_keyPredicate)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._entitySetName_keyPredicate(_entitySetName_1.Parsed, _keyPredicate_1.Parsed), _keyPredicate_1.Remainder);
                }
            }
        }
        
        public static class _singletonEntityParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._singletonEntity> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._singletonEntity>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._singletonEntity> Parse(IInput<char>? input)
                {
                    var _singletonEntity_1 = __GeneratedOdataV3.Parsers.Rules._singletonEntityParser.Instance.Parse(input);
if (!_singletonEntity_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._singletonEntity)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._singletonEntity(_singletonEntity_1.Parsed), _singletonEntity_1.Remainder);
                }
            }
        }
    }
    
}
