namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _arrayOrObjectParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._arrayOrObject> Instance { get; } = (_complexColInUriParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._arrayOrObject>(_complexInUriParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._arrayOrObject>(_rootExprColParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._arrayOrObject>(_primitiveColInUriParser.Instance);
        
        public static class _complexColInUriParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._arrayOrObject._complexColInUri> Instance { get; } = from _complexColInUri_1 in __GeneratedOdata.Parsers.Rules._complexColInUriParser.Instance
select new __GeneratedOdata.CstNodes.Rules._arrayOrObject._complexColInUri(_complexColInUri_1);
        }
        
        public static class _complexInUriParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._arrayOrObject._complexInUri> Instance { get; } = from _complexInUri_1 in __GeneratedOdata.Parsers.Rules._complexInUriParser.Instance
select new __GeneratedOdata.CstNodes.Rules._arrayOrObject._complexInUri(_complexInUri_1);
        }
        
        public static class _rootExprColParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._arrayOrObject._rootExprCol> Instance { get; } = from _rootExprCol_1 in __GeneratedOdata.Parsers.Rules._rootExprColParser.Instance
select new __GeneratedOdata.CstNodes.Rules._arrayOrObject._rootExprCol(_rootExprCol_1);
        }
        
        public static class _primitiveColInUriParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._arrayOrObject._primitiveColInUri> Instance { get; } = from _primitiveColInUri_1 in __GeneratedOdata.Parsers.Rules._primitiveColInUriParser.Instance
select new __GeneratedOdata.CstNodes.Rules._arrayOrObject._primitiveColInUri(_primitiveColInUri_1);
        }
    }
    
}
