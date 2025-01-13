namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _lambdaVariableExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._lambdaVariableExpr> Instance { get; } = from _odataIdentifier_1 in __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdata.CstNodes.Rules._lambdaVariableExpr(_odataIdentifier_1);
    }
    
}
