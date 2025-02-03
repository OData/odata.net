namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _navigationPropertyInUriParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri> Instance { get; } = (_singleNavPropInJSONParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri>(_collectionNavPropInJSONParser.Instance);
        
        public static class _singleNavPropInJSONParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._singleNavPropInJSON> Instance { get; } = from _singleNavPropInJSON_1 in __GeneratedOdataV3.Parsers.Rules._singleNavPropInJSONParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._singleNavPropInJSON(_singleNavPropInJSON_1);
        }
        
        public static class _collectionNavPropInJSONParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._collectionNavPropInJSON> Instance { get; } = from _collectionNavPropInJSON_1 in __GeneratedOdataV3.Parsers.Rules._collectionNavPropInJSONParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._collectionNavPropInJSON(_collectionNavPropInJSON_1);
        }
    }
    
}
