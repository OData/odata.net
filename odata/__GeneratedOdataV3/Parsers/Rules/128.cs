namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexColPathExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._complexColPathExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._complexColPathExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._complexColPathExpr> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_qualifiedComplexTypeName_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional().Parse(input);
if (!_ʺx2Fʺ_qualifiedComplexTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexColPathExpr)!, input);
}

var _collectionPathExpr_1 = __GeneratedOdataV3.Parsers.Rules._collectionPathExprParser.Instance.Optional().Parse(_ʺx2Fʺ_qualifiedComplexTypeName_1.Remainder);
if (!_collectionPathExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexColPathExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._complexColPathExpr(_ʺx2Fʺ_qualifiedComplexTypeName_1.Parsed.GetOrElse(null), _collectionPathExpr_1.Parsed.GetOrElse(null)), _collectionPathExpr_1.Remainder);
            }
        }
    }
    
}
