namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _identifierLeadingCharacterParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter> Instance { get; } = (_ALPHAParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter>(_ʺx5FʺParser.Instance);
        
        public static class _ALPHAParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ALPHA> Instance { get; } = from _ALPHA_1 in __GeneratedOdata.Parsers.Rules._ALPHAParser.Instance
select new __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ALPHA(_ALPHA_1);
        }
        
        public static class _ʺx5FʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ʺx5Fʺ> Instance { get; } = from _ʺx5Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx5FʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ʺx5Fʺ(_ʺx5Fʺ_1);
        }
    }
    
}