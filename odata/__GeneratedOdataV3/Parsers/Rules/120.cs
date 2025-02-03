namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _annotationExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._annotationExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._annotationExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._annotationExpr> Parse(IInput<char>? input)
            {
                var _annotation_1 = __GeneratedOdataV3.Parsers.Rules._annotationParser.Instance.Parse(input);
if (!_annotation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationExpr)!, input);
}

var _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr_1 = __GeneratedOdataV3.Parsers.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExprParser.Instance.Optional().Parse(_annotation_1.Remainder);
if (!_collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._annotationExpr(_annotation_1.Parsed,  _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr_1.Parsed.GetOrElse(null)), _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr_1.Remainder);
            }
        }
    }
    
}
