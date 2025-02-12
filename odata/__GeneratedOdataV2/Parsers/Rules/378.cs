namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _SEMIParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._SEMI> Instance { get; } = (_ʺx3BʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._SEMI>(_ʺx25x33x42ʺParser.Instance);
        
        public static class _ʺx3BʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._SEMI._ʺx3Bʺ> Instance { get; } = from _ʺx3Bʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3BʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._SEMI._ʺx3Bʺ.Instance;
        }
        
        public static class _ʺx25x33x42ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._SEMI._ʺx25x33x42ʺ> Instance { get; } = from _ʺx25x33x42ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx25x33x42ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._SEMI._ʺx25x33x42ʺ.Instance;
        }
    }
    
}
