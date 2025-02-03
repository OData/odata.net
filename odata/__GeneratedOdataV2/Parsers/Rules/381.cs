namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _OPENParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._OPEN> Instance { get; } = (_ʺx28ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._OPEN>(_ʺx25x32x38ʺParser.Instance);
        
        public static class _ʺx28ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._OPEN._ʺx28ʺ> Instance { get; } = from _ʺx28ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx28ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._OPEN._ʺx28ʺ(_ʺx28ʺ_1);
        }
        
        public static class _ʺx25x32x38ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._OPEN._ʺx25x32x38ʺ> Instance { get; } = from _ʺx25x32x38ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx25x32x38ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._OPEN._ʺx25x32x38ʺ(_ʺx25x32x38ʺ_1);
        }
    }
    
}
