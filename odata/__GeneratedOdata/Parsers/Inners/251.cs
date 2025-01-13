namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty> Instance { get; } = (_qualifiedActionNameParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty>(_qualifiedFunctionNameParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty>(_selectListPropertyParser.Instance);
        
        public static class _qualifiedActionNameParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedActionName> Instance { get; } = from _qualifiedActionName_1 in __GeneratedOdata.Parsers.Rules._qualifiedActionNameParser.Instance
select new __GeneratedOdata.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedActionName(_qualifiedActionName_1);
        }
        
        public static class _qualifiedFunctionNameParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedFunctionName> Instance { get; } = from _qualifiedFunctionName_1 in __GeneratedOdata.Parsers.Rules._qualifiedFunctionNameParser.Instance
select new __GeneratedOdata.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedFunctionName(_qualifiedFunctionName_1);
        }
        
        public static class _selectListPropertyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._selectListProperty> Instance { get; } = from _selectListProperty_1 in __GeneratedOdata.Parsers.Rules._selectListPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._selectListProperty(_selectListProperty_1);
        }
    }
    
}
