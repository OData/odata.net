namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _navigationPropertyInUriParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._navigationPropertyInUri> Instance { get; } = (_singleNavPropInJSONParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._navigationPropertyInUri>(_collectionNavPropInJSONParser.Instance);
        
        public static class _singleNavPropInJSONParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._navigationPropertyInUri._singleNavPropInJSON> Instance { get; } = from _singleNavPropInJSON_1 in __GeneratedOdata.Parsers.Rules._singleNavPropInJSONParser.Instance
select new __GeneratedOdata.CstNodes.Rules._navigationPropertyInUri._singleNavPropInJSON(_singleNavPropInJSON_1);
        }
        
        public static class _collectionNavPropInJSONParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._navigationPropertyInUri._collectionNavPropInJSON> Instance { get; } = from _collectionNavPropInJSON_1 in __GeneratedOdata.Parsers.Rules._collectionNavPropInJSONParser.Instance
select new __GeneratedOdata.CstNodes.Rules._navigationPropertyInUri._collectionNavPropInJSON(_collectionNavPropInJSON_1);
        }
    }
    
}