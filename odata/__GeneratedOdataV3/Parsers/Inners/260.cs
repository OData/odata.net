namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr> Instance { get; } = (_eqExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr>(_neExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr>(_ltExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr>(_leExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr>(_gtExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr>(_geExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr>(_hasExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr>(_inExprParser.Instance);
        
        public static class _eqExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr> Instance { get; } = from _eqExpr_1 in __GeneratedOdataV3.Parsers.Rules._eqExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr(_eqExpr_1);
        }
        
        public static class _neExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._neExpr> Instance { get; } = from _neExpr_1 in __GeneratedOdataV3.Parsers.Rules._neExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._neExpr(_neExpr_1);
        }
        
        public static class _ltExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._ltExpr> Instance { get; } = from _ltExpr_1 in __GeneratedOdataV3.Parsers.Rules._ltExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._ltExpr(_ltExpr_1);
        }
        
        public static class _leExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._leExpr> Instance { get; } = from _leExpr_1 in __GeneratedOdataV3.Parsers.Rules._leExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._leExpr(_leExpr_1);
        }
        
        public static class _gtExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._gtExpr> Instance { get; } = from _gtExpr_1 in __GeneratedOdataV3.Parsers.Rules._gtExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._gtExpr(_gtExpr_1);
        }
        
        public static class _geExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._geExpr> Instance { get; } = from _geExpr_1 in __GeneratedOdataV3.Parsers.Rules._geExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._geExpr(_geExpr_1);
        }
        
        public static class _hasExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._hasExpr> Instance { get; } = from _hasExpr_1 in __GeneratedOdataV3.Parsers.Rules._hasExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._hasExpr(_hasExpr_1);
        }
        
        public static class _inExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._inExpr> Instance { get; } = from _inExpr_1 in __GeneratedOdataV3.Parsers.Rules._inExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._inExpr(_inExpr_1);
        }
    }
    
}
