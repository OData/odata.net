namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _navigationPropertyParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._navigationProperty> Instance { get; } = (_entityNavigationPropertyParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._navigationProperty>(_entityColNavigationPropertyParser.Instance);
        
        public static class _entityNavigationPropertyParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._navigationProperty._entityNavigationProperty> Instance { get; } = from _entityNavigationProperty_1 in __GeneratedOdataV2.Parsers.Rules._entityNavigationPropertyParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._navigationProperty._entityNavigationProperty(_entityNavigationProperty_1);
        }
        
        public static class _entityColNavigationPropertyParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._navigationProperty._entityColNavigationProperty> Instance { get; } = from _entityColNavigationProperty_1 in __GeneratedOdataV2.Parsers.Rules._entityColNavigationPropertyParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._navigationProperty._entityColNavigationProperty(_entityColNavigationProperty_1);
        }
    }
    
}
