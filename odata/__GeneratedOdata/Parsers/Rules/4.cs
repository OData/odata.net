namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _resourcePathParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath> Instance { get; } = (_entitySetName_꘡collectionNavigation꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._resourcePath>(_singletonEntity_꘡singleNavigation꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._resourcePath>(_actionImportCallParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._resourcePath>(_entityColFunctionImportCall_꘡collectionNavigation꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._resourcePath>(_entityFunctionImportCall_꘡singleNavigation꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._resourcePath>(_complexColFunctionImportCall_꘡complexColPath꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._resourcePath>(_complexFunctionImportCall_꘡complexPath꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._resourcePath>(_primitiveColFunctionImportCall_꘡primitiveColPath꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._resourcePath>(_primitiveFunctionImportCall_꘡primitivePath꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._resourcePath>(_functionImportCallNoParensParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._resourcePath>(_crossjoinParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._resourcePath>(_ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser.Instance);
        
        public static class _entitySetName_꘡collectionNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡> Instance { get; } = from _entitySetName_1 in __GeneratedOdata.Parsers.Rules._entitySetNameParser.Instance
from _collectionNavigation_1 in __GeneratedOdata.Parsers.Rules._collectionNavigationParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡(_entitySetName_1, _collectionNavigation_1.GetOrElse(null));
        }
        
        public static class _singletonEntity_꘡singleNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath._singletonEntity_꘡singleNavigation꘡> Instance { get; } = from _singletonEntity_1 in __GeneratedOdata.Parsers.Rules._singletonEntityParser.Instance
from _singleNavigation_1 in __GeneratedOdata.Parsers.Rules._singleNavigationParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._resourcePath._singletonEntity_꘡singleNavigation꘡(_singletonEntity_1, _singleNavigation_1.GetOrElse(null));
        }
        
        public static class _actionImportCallParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath._actionImportCall> Instance { get; } = from _actionImportCall_1 in __GeneratedOdata.Parsers.Rules._actionImportCallParser.Instance
select new __GeneratedOdata.CstNodes.Rules._resourcePath._actionImportCall(_actionImportCall_1);
        }
        
        public static class _entityColFunctionImportCall_꘡collectionNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath._entityColFunctionImportCall_꘡collectionNavigation꘡> Instance { get; } = from _entityColFunctionImportCall_1 in __GeneratedOdata.Parsers.Rules._entityColFunctionImportCallParser.Instance
from _collectionNavigation_1 in __GeneratedOdata.Parsers.Rules._collectionNavigationParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._resourcePath._entityColFunctionImportCall_꘡collectionNavigation꘡(_entityColFunctionImportCall_1, _collectionNavigation_1.GetOrElse(null));
        }
        
        public static class _entityFunctionImportCall_꘡singleNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath._entityFunctionImportCall_꘡singleNavigation꘡> Instance { get; } = from _entityFunctionImportCall_1 in __GeneratedOdata.Parsers.Rules._entityFunctionImportCallParser.Instance
from _singleNavigation_1 in __GeneratedOdata.Parsers.Rules._singleNavigationParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._resourcePath._entityFunctionImportCall_꘡singleNavigation꘡(_entityFunctionImportCall_1, _singleNavigation_1.GetOrElse(null));
        }
        
        public static class _complexColFunctionImportCall_꘡complexColPath꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath._complexColFunctionImportCall_꘡complexColPath꘡> Instance { get; } = from _complexColFunctionImportCall_1 in __GeneratedOdata.Parsers.Rules._complexColFunctionImportCallParser.Instance
from _complexColPath_1 in __GeneratedOdata.Parsers.Rules._complexColPathParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._resourcePath._complexColFunctionImportCall_꘡complexColPath꘡(_complexColFunctionImportCall_1, _complexColPath_1.GetOrElse(null));
        }
        
        public static class _complexFunctionImportCall_꘡complexPath꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath._complexFunctionImportCall_꘡complexPath꘡> Instance { get; } = from _complexFunctionImportCall_1 in __GeneratedOdata.Parsers.Rules._complexFunctionImportCallParser.Instance
from _complexPath_1 in __GeneratedOdata.Parsers.Rules._complexPathParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._resourcePath._complexFunctionImportCall_꘡complexPath꘡(_complexFunctionImportCall_1, _complexPath_1.GetOrElse(null));
        }
        
        public static class _primitiveColFunctionImportCall_꘡primitiveColPath꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath._primitiveColFunctionImportCall_꘡primitiveColPath꘡> Instance { get; } = from _primitiveColFunctionImportCall_1 in __GeneratedOdata.Parsers.Rules._primitiveColFunctionImportCallParser.Instance
from _primitiveColPath_1 in __GeneratedOdata.Parsers.Rules._primitiveColPathParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._resourcePath._primitiveColFunctionImportCall_꘡primitiveColPath꘡(_primitiveColFunctionImportCall_1, _primitiveColPath_1.GetOrElse(null));
        }
        
        public static class _primitiveFunctionImportCall_꘡primitivePath꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath._primitiveFunctionImportCall_꘡primitivePath꘡> Instance { get; } = from _primitiveFunctionImportCall_1 in __GeneratedOdata.Parsers.Rules._primitiveFunctionImportCallParser.Instance
from _primitivePath_1 in __GeneratedOdata.Parsers.Rules._primitivePathParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._resourcePath._primitiveFunctionImportCall_꘡primitivePath꘡(_primitiveFunctionImportCall_1, _primitivePath_1.GetOrElse(null));
        }
        
        public static class _functionImportCallNoParensParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath._functionImportCallNoParens> Instance { get; } = from _functionImportCallNoParens_1 in __GeneratedOdata.Parsers.Rules._functionImportCallNoParensParser.Instance
select new __GeneratedOdata.CstNodes.Rules._resourcePath._functionImportCallNoParens(_functionImportCallNoParens_1);
        }
        
        public static class _crossjoinParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath._crossjoin> Instance { get; } = from _crossjoin_1 in __GeneratedOdata.Parsers.Rules._crossjoinParser.Instance
select new __GeneratedOdata.CstNodes.Rules._resourcePath._crossjoin(_crossjoin_1);
        }
        
        public static class _ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._resourcePath._ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡> Instance { get; } = from _ʺx24x61x6Cx6Cʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x61x6Cx6CʺParser.Instance
from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._resourcePath._ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡(_ʺx24x61x6Cx6Cʺ_1, _ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null));
        }
    }
    
}
