namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName> Instance { get; } = (_selectPropertyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName>(_qualifiedActionNameParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName>(_qualifiedFunctionNameParser.Instance);
        
        public static class _selectPropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._selectProperty> Instance { get; } = from _selectProperty_1 in __GeneratedOdataV3.Parsers.Rules._selectPropertyParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._selectProperty(_selectProperty_1);
        }
        
        public static class _qualifiedActionNameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedActionName> Instance { get; } = from _qualifiedActionName_1 in __GeneratedOdataV3.Parsers.Rules._qualifiedActionNameParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedActionName(_qualifiedActionName_1);
        }
        
        public static class _qualifiedFunctionNameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedFunctionName> Instance { get; } = from _qualifiedFunctionName_1 in __GeneratedOdataV3.Parsers.Rules._qualifiedFunctionNameParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedFunctionName(_qualifiedFunctionName_1);
        }
    }
    
}
