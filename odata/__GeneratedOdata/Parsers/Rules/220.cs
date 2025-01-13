namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _intParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._int> Instance { get; } = (_ʺx30ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._int>(_ⲤoneToNine_ЖDIGITↃParser.Instance);
        
        public static class _ʺx30ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._int._ʺx30ʺ> Instance { get; } = from _ʺx30ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx30ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._int._ʺx30ʺ(_ʺx30ʺ_1);
        }
        
        public static class _ⲤoneToNine_ЖDIGITↃParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._int._ⲤoneToNine_ЖDIGITↃ> Instance { get; } = from _ⲤoneToNine_ЖDIGITↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤoneToNine_ЖDIGITↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._int._ⲤoneToNine_ЖDIGITↃ(_ⲤoneToNine_ЖDIGITↃ_1);
        }
    }
    
}
