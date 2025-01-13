namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _identifierCharacterParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._identifierCharacter> Instance { get; } = (_ALPHAParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._identifierCharacter>(_ʺx5FʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._identifierCharacter>(_DIGITParser.Instance);
        
        public static class _ALPHAParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._identifierCharacter._ALPHA> Instance { get; } = from _ALPHA_1 in __GeneratedOdata.Parsers.Rules._ALPHAParser.Instance
select new __GeneratedOdata.CstNodes.Rules._identifierCharacter._ALPHA(_ALPHA_1);
        }
        
        public static class _ʺx5FʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._identifierCharacter._ʺx5Fʺ> Instance { get; } = from _ʺx5Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx5FʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._identifierCharacter._ʺx5Fʺ(_ʺx5Fʺ_1);
        }
        
        public static class _DIGITParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._identifierCharacter._DIGIT> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdata.CstNodes.Rules._identifierCharacter._DIGIT(_DIGIT_1);
        }
    }
    
}
