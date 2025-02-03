namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionImportCallNoParensParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens> Instance { get; } = (_entityFunctionImportParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens>(_entityColFunctionImportParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens>(_complexFunctionImportParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens>(_complexColFunctionImportParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens>(_primitiveFunctionImportParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens>(_primitiveColFunctionImportParser.Instance);
        
        public static class _entityFunctionImportParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens._entityFunctionImport> Instance { get; } = from _entityFunctionImport_1 in __GeneratedOdataV2.Parsers.Rules._entityFunctionImportParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens._entityFunctionImport(_entityFunctionImport_1);
        }
        
        public static class _entityColFunctionImportParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens._entityColFunctionImport> Instance { get; } = from _entityColFunctionImport_1 in __GeneratedOdataV2.Parsers.Rules._entityColFunctionImportParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens._entityColFunctionImport(_entityColFunctionImport_1);
        }
        
        public static class _complexFunctionImportParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens._complexFunctionImport> Instance { get; } = from _complexFunctionImport_1 in __GeneratedOdataV2.Parsers.Rules._complexFunctionImportParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens._complexFunctionImport(_complexFunctionImport_1);
        }
        
        public static class _complexColFunctionImportParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens._complexColFunctionImport> Instance { get; } = from _complexColFunctionImport_1 in __GeneratedOdataV2.Parsers.Rules._complexColFunctionImportParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens._complexColFunctionImport(_complexColFunctionImport_1);
        }
        
        public static class _primitiveFunctionImportParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens._primitiveFunctionImport> Instance { get; } = from _primitiveFunctionImport_1 in __GeneratedOdataV2.Parsers.Rules._primitiveFunctionImportParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens._primitiveFunctionImport(_primitiveFunctionImport_1);
        }
        
        public static class _primitiveColFunctionImportParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens._primitiveColFunctionImport> Instance { get; } = from _primitiveColFunctionImport_1 in __GeneratedOdataV2.Parsers.Rules._primitiveColFunctionImportParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._functionImportCallNoParens._primitiveColFunctionImport(_primitiveColFunctionImport_1);
        }
    }
    
}
