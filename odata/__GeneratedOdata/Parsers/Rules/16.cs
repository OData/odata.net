namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _propertyPathParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._propertyPath> Instance { get; } = (_entityColNavigationProperty_꘡collectionNavigation꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._propertyPath>(_entityNavigationProperty_꘡singleNavigation꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._propertyPath>(_complexColProperty_꘡complexColPath꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._propertyPath>(_complexProperty_꘡complexPath꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._propertyPath>(_primitiveColProperty_꘡primitiveColPath꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._propertyPath>(_primitiveProperty_꘡primitivePath꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._propertyPath>(_streamProperty_꘡boundOperation꘡Parser.Instance);
        
        public static class _entityColNavigationProperty_꘡collectionNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._propertyPath._entityColNavigationProperty_꘡collectionNavigation꘡> Instance { get; } = from _entityColNavigationProperty_1 in __GeneratedOdata.Parsers.Rules._entityColNavigationPropertyParser.Instance
from _collectionNavigation_1 in __GeneratedOdata.Parsers.Rules._collectionNavigationParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._propertyPath._entityColNavigationProperty_꘡collectionNavigation꘡(_entityColNavigationProperty_1, _collectionNavigation_1.GetOrElse(null));
        }
        
        public static class _entityNavigationProperty_꘡singleNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._propertyPath._entityNavigationProperty_꘡singleNavigation꘡> Instance { get; } = from _entityNavigationProperty_1 in __GeneratedOdata.Parsers.Rules._entityNavigationPropertyParser.Instance
from _singleNavigation_1 in __GeneratedOdata.Parsers.Rules._singleNavigationParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._propertyPath._entityNavigationProperty_꘡singleNavigation꘡(_entityNavigationProperty_1, _singleNavigation_1.GetOrElse(null));
        }
        
        public static class _complexColProperty_꘡complexColPath꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._propertyPath._complexColProperty_꘡complexColPath꘡> Instance { get; } = from _complexColProperty_1 in __GeneratedOdata.Parsers.Rules._complexColPropertyParser.Instance
from _complexColPath_1 in __GeneratedOdata.Parsers.Rules._complexColPathParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._propertyPath._complexColProperty_꘡complexColPath꘡(_complexColProperty_1, _complexColPath_1.GetOrElse(null));
        }
        
        public static class _complexProperty_꘡complexPath꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._propertyPath._complexProperty_꘡complexPath꘡> Instance { get; } = from _complexProperty_1 in __GeneratedOdata.Parsers.Rules._complexPropertyParser.Instance
from _complexPath_1 in __GeneratedOdata.Parsers.Rules._complexPathParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._propertyPath._complexProperty_꘡complexPath꘡(_complexProperty_1, _complexPath_1.GetOrElse(null));
        }
        
        public static class _primitiveColProperty_꘡primitiveColPath꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._propertyPath._primitiveColProperty_꘡primitiveColPath꘡> Instance { get; } = from _primitiveColProperty_1 in __GeneratedOdata.Parsers.Rules._primitiveColPropertyParser.Instance
from _primitiveColPath_1 in __GeneratedOdata.Parsers.Rules._primitiveColPathParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._propertyPath._primitiveColProperty_꘡primitiveColPath꘡(_primitiveColProperty_1, _primitiveColPath_1.GetOrElse(null));
        }
        
        public static class _primitiveProperty_꘡primitivePath꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._propertyPath._primitiveProperty_꘡primitivePath꘡> Instance { get; } = from _primitiveProperty_1 in __GeneratedOdata.Parsers.Rules._primitivePropertyParser.Instance
from _primitivePath_1 in __GeneratedOdata.Parsers.Rules._primitivePathParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._propertyPath._primitiveProperty_꘡primitivePath꘡(_primitiveProperty_1, _primitivePath_1.GetOrElse(null));
        }
        
        public static class _streamProperty_꘡boundOperation꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._propertyPath._streamProperty_꘡boundOperation꘡> Instance { get; } = from _streamProperty_1 in __GeneratedOdata.Parsers.Rules._streamPropertyParser.Instance
from _boundOperation_1 in __GeneratedOdata.Parsers.Rules._boundOperationParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._propertyPath._streamProperty_꘡boundOperation꘡(_streamProperty_1, _boundOperation_1.GetOrElse(null));
        }
    }
    
}
