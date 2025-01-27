namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _qualifiedEntityTypeNameⳆqualifiedComplexTypeNameParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName> Instance { get; } = (_qualifiedEntityTypeNameParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName>(_qualifiedComplexTypeNameParser.Instance);
        
        public static class _qualifiedEntityTypeNameParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedEntityTypeName> Instance { get; } = from _qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Rules._qualifiedEntityTypeNameParser.Instance
select new __GeneratedOdata.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedEntityTypeName(_qualifiedEntityTypeName_1);
        }
        
        public static class _qualifiedComplexTypeNameParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedComplexTypeName> Instance { get; } = from _qualifiedComplexTypeName_1 in __GeneratedOdata.Parsers.Rules._qualifiedComplexTypeNameParser.Instance
select new __GeneratedOdata.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedComplexTypeName(_qualifiedComplexTypeName_1);
        }
    }
    
}
