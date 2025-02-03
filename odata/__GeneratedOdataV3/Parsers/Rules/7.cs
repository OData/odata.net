namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyPredicateParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._keyPredicate> Instance { get; } = (_simpleKeyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._keyPredicate>(_compoundKeyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._keyPredicate>(_keyPathSegmentsParser.Instance);
        
        public static class _simpleKeyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._keyPredicate._simpleKey> Instance { get; } = from _simpleKey_1 in __GeneratedOdataV3.Parsers.Rules._simpleKeyParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._keyPredicate._simpleKey(_simpleKey_1);
        }
        
        public static class _compoundKeyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._keyPredicate._compoundKey> Instance { get; } = from _compoundKey_1 in __GeneratedOdataV3.Parsers.Rules._compoundKeyParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._keyPredicate._compoundKey(_compoundKey_1);
        }
        
        public static class _keyPathSegmentsParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._keyPredicate._keyPathSegments> Instance { get; } = from _keyPathSegments_1 in __GeneratedOdataV3.Parsers.Rules._keyPathSegmentsParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._keyPredicate._keyPathSegments(_keyPathSegments_1);
        }
    }
    
}
