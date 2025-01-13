namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _primitivePropertyParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveProperty> Instance { get; } = (_primitiveKeyPropertyParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveProperty>(_primitiveNonKeyPropertyParser.Instance);
        
        public static class _primitiveKeyPropertyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveProperty._primitiveKeyProperty> Instance { get; } = from _primitiveKeyProperty_1 in __GeneratedOdata.Parsers.Rules._primitiveKeyPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveProperty._primitiveKeyProperty(_primitiveKeyProperty_1);
        }
        
        public static class _primitiveNonKeyPropertyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveProperty._primitiveNonKeyProperty> Instance { get; } = from _primitiveNonKeyProperty_1 in __GeneratedOdata.Parsers.Rules._primitiveNonKeyPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveProperty._primitiveNonKeyProperty(_primitiveNonKeyProperty_1);
        }
    }
    
}
