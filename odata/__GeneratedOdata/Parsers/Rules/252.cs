namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _navigationPropertyParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._navigationProperty> Instance { get; } = (_entityNavigationPropertyParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._navigationProperty>(_entityColNavigationPropertyParser.Instance);
        
        public static class _entityNavigationPropertyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._navigationProperty._entityNavigationProperty> Instance { get; } = from _entityNavigationProperty_1 in __GeneratedOdata.Parsers.Rules._entityNavigationPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._navigationProperty._entityNavigationProperty(_entityNavigationProperty_1);
        }
        
        public static class _entityColNavigationPropertyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._navigationProperty._entityColNavigationProperty> Instance { get; } = from _entityColNavigationProperty_1 in __GeneratedOdata.Parsers.Rules._entityColNavigationPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._navigationProperty._entityColNavigationProperty(_entityColNavigationProperty_1);
        }
    }
    
}
