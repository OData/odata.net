namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _parameterValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._parameterValue> Instance { get; } = (_arrayOrObjectParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._parameterValue>(_commonExprParser.Instance);
        
        public static class _arrayOrObjectParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._parameterValue._arrayOrObject> Instance { get; } = from _arrayOrObject_1 in __GeneratedOdataV2.Parsers.Rules._arrayOrObjectParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._parameterValue._arrayOrObject(_arrayOrObject_1);
        }
        
        public static class _commonExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._parameterValue._commonExpr> Instance { get; } = from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._parameterValue._commonExpr(_commonExpr_1);
        }
    }
    
}
