namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _contextPropertyPathParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._contextPropertyPath> Instance { get; } = (_primitivePropertyParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._contextPropertyPath>(_primitiveColPropertyParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._contextPropertyPath>(_complexColPropertyParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._contextPropertyPath>(_complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡Parser.Instance);
        
        public static class _primitivePropertyParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._contextPropertyPath._primitiveProperty> Instance { get; } = from _primitiveProperty_1 in __GeneratedOdata.Parsers.Rules._primitivePropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._contextPropertyPath._primitiveProperty(_primitiveProperty_1);
        }
        
        public static class _primitiveColPropertyParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._contextPropertyPath._primitiveColProperty> Instance { get; } = from _primitiveColProperty_1 in __GeneratedOdata.Parsers.Rules._primitiveColPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._contextPropertyPath._primitiveColProperty(_primitiveColProperty_1);
        }
        
        public static class _complexColPropertyParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._contextPropertyPath._complexColProperty> Instance { get; } = from _complexColProperty_1 in __GeneratedOdata.Parsers.Rules._complexColPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._contextPropertyPath._complexColProperty(_complexColProperty_1);
        }
        
        public static class _complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._contextPropertyPath._complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡> Instance { get; } = from _complexProperty_1 in __GeneratedOdata.Parsers.Rules._complexPropertyParser.Instance
from _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath_1 in __GeneratedOdata.Parsers.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPathParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._contextPropertyPath._complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡(_complexProperty_1, _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath_1.GetOrElse(null));
        }
    }
    
}
