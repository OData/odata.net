namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitivePropertyParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveProperty> Instance { get; } = (_primitiveKeyPropertyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveProperty>(_primitiveNonKeyPropertyParser.Instance);
        
        public static class _primitiveKeyPropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveKeyProperty> Instance { get; } = from _primitiveKeyProperty_1 in __GeneratedOdataV3.Parsers.Rules._primitiveKeyPropertyParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveKeyProperty(_primitiveKeyProperty_1);
        }
        
        public static class _primitiveNonKeyPropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveNonKeyProperty> Instance { get; } = from _primitiveNonKeyProperty_1 in __GeneratedOdataV3.Parsers.Rules._primitiveNonKeyPropertyParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveNonKeyProperty(_primitiveNonKeyProperty_1);
        }
    }
    
}
