namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty> Instance { get; } = (_qualifiedActionNameParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty>(_qualifiedFunctionNameParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty>(_selectListPropertyParser.Instance);
        
        public static class _qualifiedActionNameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedActionName> Instance { get; } = from _qualifiedActionName_1 in __GeneratedOdataV3.Parsers.Rules._qualifiedActionNameParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedActionName(_qualifiedActionName_1);
        }
        
        public static class _qualifiedFunctionNameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedFunctionName> Instance { get; } = from _qualifiedFunctionName_1 in __GeneratedOdataV3.Parsers.Rules._qualifiedFunctionNameParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedFunctionName(_qualifiedFunctionName_1);
        }
        
        public static class _selectListPropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._selectListProperty> Instance { get; } = from _selectListProperty_1 in __GeneratedOdataV3.Parsers.Rules._selectListPropertyParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._selectListProperty(_selectListProperty_1);
        }
    }
    
}
