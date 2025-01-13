namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _selectPropertyParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._selectProperty> Instance { get; } = (_primitivePropertyParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectProperty>(_primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡Parser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectProperty>(_navigationPropertyParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._selectProperty>(_selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡Parser.Instance);
        
        public static class _primitivePropertyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectProperty._primitiveProperty> Instance { get; } = from _primitiveProperty_1 in __GeneratedOdata.Parsers.Rules._primitivePropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectProperty._primitiveProperty(_primitiveProperty_1);
        }
        
        public static class _primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectProperty._primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡> Instance { get; } = from _primitiveColProperty_1 in __GeneratedOdata.Parsers.Rules._primitiveColPropertyParser.Instance
from _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE_1 in __GeneratedOdata.Parsers.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSEParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._selectProperty._primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡(_primitiveColProperty_1, _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE_1.GetOrElse(null));
        }
        
        public static class _navigationPropertyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectProperty._navigationProperty> Instance { get; } = from _navigationProperty_1 in __GeneratedOdata.Parsers.Rules._navigationPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._selectProperty._navigationProperty(_navigationProperty_1);
        }
        
        public static class _selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._selectProperty._selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡> Instance { get; } = from _selectPath_1 in __GeneratedOdata.Parsers.Rules._selectPathParser.Instance
from _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty_1 in __GeneratedOdata.Parsers.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectPropertyParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._selectProperty._selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡(_selectPath_1, _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty_1.GetOrElse(null));
        }
    }
    
}
