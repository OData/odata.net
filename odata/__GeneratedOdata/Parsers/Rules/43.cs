namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _functionImportCallNoParensParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens> Instance { get; } = (_entityFunctionImportParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens>(_entityColFunctionImportParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens>(_complexFunctionImportParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens>(_complexColFunctionImportParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens>(_primitiveFunctionImportParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens>(_primitiveColFunctionImportParser.Instance);
        
        public static class _entityFunctionImportParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._entityFunctionImport> Instance { get; } = from _entityFunctionImport_1 in __GeneratedOdata.Parsers.Rules._entityFunctionImportParser.Instance
select new __GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._entityFunctionImport(_entityFunctionImport_1);
        }
        
        public static class _entityColFunctionImportParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._entityColFunctionImport> Instance { get; } = from _entityColFunctionImport_1 in __GeneratedOdata.Parsers.Rules._entityColFunctionImportParser.Instance
select new __GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._entityColFunctionImport(_entityColFunctionImport_1);
        }
        
        public static class _complexFunctionImportParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._complexFunctionImport> Instance { get; } = from _complexFunctionImport_1 in __GeneratedOdata.Parsers.Rules._complexFunctionImportParser.Instance
select new __GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._complexFunctionImport(_complexFunctionImport_1);
        }
        
        public static class _complexColFunctionImportParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._complexColFunctionImport> Instance { get; } = from _complexColFunctionImport_1 in __GeneratedOdata.Parsers.Rules._complexColFunctionImportParser.Instance
select new __GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._complexColFunctionImport(_complexColFunctionImport_1);
        }
        
        public static class _primitiveFunctionImportParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._primitiveFunctionImport> Instance { get; } = from _primitiveFunctionImport_1 in __GeneratedOdata.Parsers.Rules._primitiveFunctionImportParser.Instance
select new __GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._primitiveFunctionImport(_primitiveFunctionImport_1);
        }
        
        public static class _primitiveColFunctionImportParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._primitiveColFunctionImport> Instance { get; } = from _primitiveColFunctionImport_1 in __GeneratedOdata.Parsers.Rules._primitiveColFunctionImportParser.Instance
select new __GeneratedOdata.CstNodes.Rules._functionImportCallNoParens._primitiveColFunctionImport(_primitiveColFunctionImport_1);
        }
    }
    
}
