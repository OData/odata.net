namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _parameterValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._parameterValue> Instance { get; } = (_arrayOrObjectParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._parameterValue>(_commonExprParser.Instance);
        
        public static class _arrayOrObjectParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._parameterValue._arrayOrObject> Instance { get; } = from _arrayOrObject_1 in __GeneratedOdata.Parsers.Rules._arrayOrObjectParser.Instance
select new __GeneratedOdata.CstNodes.Rules._parameterValue._arrayOrObject(_arrayOrObject_1);
        }
        
        public static class _commonExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._parameterValue._commonExpr> Instance { get; } = from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._parameterValue._commonExpr(_commonExpr_1);
        }
    }
    
}
