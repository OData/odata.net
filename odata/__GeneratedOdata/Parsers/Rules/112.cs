namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _selectListPropertyParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._selectListProperty> Instance { get; } = (_primitivePropertyParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectListProperty>(_primitiveColPropertyParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectListProperty>(_navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡Parser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectListProperty>(_selectPath_꘡ʺx2Fʺ_selectListProperty꘡Parser.Instance);
        
        public static class _primitivePropertyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectListProperty._primitiveProperty> Instance { get; } = from _primitiveProperty_1 in __GeneratedOdata.Parsers.Rules._primitivePropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectListProperty._primitiveProperty(_primitiveProperty_1);
        }
        
        public static class _primitiveColPropertyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectListProperty._primitiveColProperty> Instance { get; } = from _primitiveColProperty_1 in __GeneratedOdata.Parsers.Rules._primitiveColPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectListProperty._primitiveColProperty(_primitiveColProperty_1);
        }
        
        public static class _navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectListProperty._navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡> Instance { get; } = from _navigationProperty_1 in __GeneratedOdata.Parsers.Rules._navigationPropertyParser.Instance
from _ʺx2Bʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2BʺParser.Instance.Optional()
from _selectList_1 in __GeneratedOdata.Parsers.Rules._selectListParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._selectListProperty._navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡(_navigationProperty_1, _ʺx2Bʺ_1.GetOrElse(null), _selectList_1.GetOrElse(null));
        }
        
        public static class _selectPath_꘡ʺx2Fʺ_selectListProperty꘡Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectListProperty._selectPath_꘡ʺx2Fʺ_selectListProperty꘡> Instance { get; } = from _selectPath_1 in __GeneratedOdata.Parsers.Rules._selectPathParser.Instance
from _ʺx2Fʺ_selectListProperty_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_selectListPropertyParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._selectListProperty._selectPath_꘡ʺx2Fʺ_selectListProperty꘡(_selectPath_1, _ʺx2Fʺ_selectListProperty_1.GetOrElse(null));
        }
    }
    
}