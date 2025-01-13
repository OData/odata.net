namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _entitySetName_keyPredicateⳆsingletonEntityParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity> Instance { get; } = (_entitySetName_keyPredicateParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity>(_singletonEntityParser.Instance);
        
        public static class _entitySetName_keyPredicateParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._entitySetName_keyPredicate> Instance { get; } = from _entitySetName_1 in __GeneratedOdata.Parsers.Rules._entitySetNameParser.Instance
from _keyPredicate_1 in __GeneratedOdata.Parsers.Rules._keyPredicateParser.Instance
select new __GeneratedOdata.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._entitySetName_keyPredicate(_entitySetName_1, _keyPredicate_1);
        }
        
        public static class _singletonEntityParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._singletonEntity> Instance { get; } = from _singletonEntity_1 in __GeneratedOdata.Parsers.Rules._singletonEntityParser.Instance
select new __GeneratedOdata.CstNodes.Inners._entitySetName_keyPredicateⳆsingletonEntity._singletonEntity(_singletonEntity_1);
        }
    }
    
}
