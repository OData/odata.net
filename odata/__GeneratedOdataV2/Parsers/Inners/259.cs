namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr> Instance { get; } = (_addExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr>(_subExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr>(_mulExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr>(_divExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr>(_divbyExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr>(_modExprParser.Instance);
        
        public static class _addExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._addExpr> Instance { get; } = from _addExpr_1 in __GeneratedOdataV2.Parsers.Rules._addExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._addExpr(_addExpr_1);
        }
        
        public static class _subExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._subExpr> Instance { get; } = from _subExpr_1 in __GeneratedOdataV2.Parsers.Rules._subExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._subExpr(_subExpr_1);
        }
        
        public static class _mulExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._mulExpr> Instance { get; } = from _mulExpr_1 in __GeneratedOdataV2.Parsers.Rules._mulExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._mulExpr(_mulExpr_1);
        }
        
        public static class _divExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divExpr> Instance { get; } = from _divExpr_1 in __GeneratedOdataV2.Parsers.Rules._divExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divExpr(_divExpr_1);
        }
        
        public static class _divbyExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divbyExpr> Instance { get; } = from _divbyExpr_1 in __GeneratedOdataV2.Parsers.Rules._divbyExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divbyExpr(_divbyExpr_1);
        }
        
        public static class _modExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._modExpr> Instance { get; } = from _modExpr_1 in __GeneratedOdataV2.Parsers.Rules._modExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._modExpr(_modExpr_1);
        }
    }
    
}
