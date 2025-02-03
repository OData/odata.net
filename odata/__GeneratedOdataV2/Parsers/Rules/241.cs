namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _identifierCharacterParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._identifierCharacter> Instance { get; } = (_ALPHAParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._identifierCharacter>(_ʺx5FʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._identifierCharacter>(_DIGITParser.Instance);
        
        public static class _ALPHAParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._identifierCharacter._ALPHA> Instance { get; } = from _ALPHA_1 in __GeneratedOdataV2.Parsers.Rules._ALPHAParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._identifierCharacter._ALPHA(_ALPHA_1);
        }
        
        public static class _ʺx5FʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._identifierCharacter._ʺx5Fʺ> Instance { get; } = from _ʺx5Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx5FʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._identifierCharacter._ʺx5Fʺ(_ʺx5Fʺ_1);
        }
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._identifierCharacter._DIGIT> Instance { get; } = from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._identifierCharacter._DIGIT(_DIGIT_1);
        }
    }
    
}
