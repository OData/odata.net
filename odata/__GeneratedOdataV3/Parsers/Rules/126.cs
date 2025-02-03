namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionNavigationExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavigationExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavigationExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavigationExpr> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_qualifiedEntityTypeName_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional().Parse(input);
if (!_ʺx2Fʺ_qualifiedEntityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavigationExpr)!, input);
}

var _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1 = __GeneratedOdataV3.Parsers.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExprParser.Instance.Optional().Parse(_ʺx2Fʺ_qualifiedEntityTypeName_1.Remainder);
if (!_keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavigationExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._collectionNavigationExpr(_ʺx2Fʺ_qualifiedEntityTypeName_1.Parsed.GetOrElse(null),  _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1.Parsed.GetOrElse(null)), _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1.Remainder);
            }
        }
    }
    
}
