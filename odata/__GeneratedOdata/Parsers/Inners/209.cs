namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName> Instance { get; } = (_selectPropertyParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName>(_qualifiedActionNameParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName>(_qualifiedFunctionNameParser.Instance);
        
        public static class _selectPropertyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._selectProperty> Instance { get; } = from _selectProperty_1 in __GeneratedOdata.Parsers.Rules._selectPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._selectProperty(_selectProperty_1);
        }
        
        public static class _qualifiedActionNameParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedActionName> Instance { get; } = from _qualifiedActionName_1 in __GeneratedOdata.Parsers.Rules._qualifiedActionNameParser.Instance
select new __GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedActionName(_qualifiedActionName_1);
        }
        
        public static class _qualifiedFunctionNameParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedFunctionName> Instance { get; } = from _qualifiedFunctionName_1 in __GeneratedOdata.Parsers.Rules._qualifiedFunctionNameParser.Instance
select new __GeneratedOdata.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedFunctionName(_qualifiedFunctionName_1);
        }
    }
    
}
