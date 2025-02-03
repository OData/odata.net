namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexPathExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._complexPathExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._complexPathExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._complexPathExpr> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_qualifiedComplexTypeName_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional().Parse(input);
if (!_ʺx2Fʺ_qualifiedComplexTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexPathExpr)!, input);
}

var _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExprParser.Instance.Optional().Parse(_ʺx2Fʺ_qualifiedComplexTypeName_1.Remainder);
if (!_ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexPathExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._complexPathExpr(_ʺx2Fʺ_qualifiedComplexTypeName_1.Parsed.GetOrElse(null),  _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr_1.Parsed.GetOrElse(null)), _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr_1.Remainder);
            }
        }
    }
    
}
