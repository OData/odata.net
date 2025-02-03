namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _arrayOrObjectParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._arrayOrObject> Instance { get; } = (_complexColInUriParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._arrayOrObject>(_complexInUriParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._arrayOrObject>(_rootExprColParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._arrayOrObject>(_primitiveColInUriParser.Instance);
        
        public static class _complexColInUriParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._arrayOrObject._complexColInUri> Instance { get; } = from _complexColInUri_1 in __GeneratedOdataV2.Parsers.Rules._complexColInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._arrayOrObject._complexColInUri(_complexColInUri_1);
        }
        
        public static class _complexInUriParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._arrayOrObject._complexInUri> Instance { get; } = from _complexInUri_1 in __GeneratedOdataV2.Parsers.Rules._complexInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._arrayOrObject._complexInUri(_complexInUri_1);
        }
        
        public static class _rootExprColParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._arrayOrObject._rootExprCol> Instance { get; } = from _rootExprCol_1 in __GeneratedOdataV2.Parsers.Rules._rootExprColParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._arrayOrObject._rootExprCol(_rootExprCol_1);
        }
        
        public static class _primitiveColInUriParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._arrayOrObject._primitiveColInUri> Instance { get; } = from _primitiveColInUri_1 in __GeneratedOdataV2.Parsers.Rules._primitiveColInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._arrayOrObject._primitiveColInUri(_primitiveColInUri_1);
        }
    }
    
}
