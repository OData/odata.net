namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _identifierLeadingCharacterParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._identifierLeadingCharacter> Instance { get; } = (_ALPHAParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._identifierLeadingCharacter>(_ʺx5FʺParser.Instance);
        
        public static class _ALPHAParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._identifierLeadingCharacter._ALPHA> Instance { get; } = from _ALPHA_1 in __GeneratedOdataV2.Parsers.Rules._ALPHAParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._identifierLeadingCharacter._ALPHA(_ALPHA_1);
        }
        
        public static class _ʺx5FʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._identifierLeadingCharacter._ʺx5Fʺ> Instance { get; } = from _ʺx5Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx5FʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._identifierLeadingCharacter._ʺx5Fʺ(_ʺx5Fʺ_1);
        }
    }
    
}
