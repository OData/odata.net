namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleQualifiedTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName> Instance { get; } = (_qualifiedEntityTypeNameParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName>(_qualifiedComplexTypeNameParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName>(_qualifiedTypeDefinitionNameParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName>(_qualifiedEnumTypeNameParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName>(_primitiveTypeNameParser.Instance);
        
        public static class _qualifiedEntityTypeNameParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName._qualifiedEntityTypeName> Instance { get; } = from _qualifiedEntityTypeName_1 in __GeneratedOdataV2.Parsers.Rules._qualifiedEntityTypeNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName._qualifiedEntityTypeName(_qualifiedEntityTypeName_1);
        }
        
        public static class _qualifiedComplexTypeNameParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName._qualifiedComplexTypeName> Instance { get; } = from _qualifiedComplexTypeName_1 in __GeneratedOdataV2.Parsers.Rules._qualifiedComplexTypeNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName._qualifiedComplexTypeName(_qualifiedComplexTypeName_1);
        }
        
        public static class _qualifiedTypeDefinitionNameParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName._qualifiedTypeDefinitionName> Instance { get; } = from _qualifiedTypeDefinitionName_1 in __GeneratedOdataV2.Parsers.Rules._qualifiedTypeDefinitionNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName._qualifiedTypeDefinitionName(_qualifiedTypeDefinitionName_1);
        }
        
        public static class _qualifiedEnumTypeNameParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName._qualifiedEnumTypeName> Instance { get; } = from _qualifiedEnumTypeName_1 in __GeneratedOdataV2.Parsers.Rules._qualifiedEnumTypeNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName._qualifiedEnumTypeName(_qualifiedEnumTypeName_1);
        }
        
        public static class _primitiveTypeNameParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName._primitiveTypeName> Instance { get; } = from _primitiveTypeName_1 in __GeneratedOdataV2.Parsers.Rules._primitiveTypeNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._singleQualifiedTypeName._primitiveTypeName(_primitiveTypeName_1);
        }
    }
    
}
