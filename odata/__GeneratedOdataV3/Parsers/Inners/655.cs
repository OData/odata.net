namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _pcharⳆʺx2FʺⳆʺx3FʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ> Instance { get; } = (_pcharParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ>(_ʺx2FʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ>(_ʺx3FʺParser.Instance);
        
        public static class _pcharParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._pchar> Instance { get; } = from _pchar_1 in __GeneratedOdataV3.Parsers.Rules._pcharParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._pchar(_pchar_1);
        }
        
        public static class _ʺx2FʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx2Fʺ> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx2Fʺ(_ʺx2Fʺ_1);
        }
        
        public static class _ʺx3FʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx3Fʺ> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx3FʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx3Fʺ(_ʺx3Fʺ_1);
        }
    }
    
}
