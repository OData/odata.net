namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _intParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._int> Instance { get; } = (_ʺx30ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._int>(_ⲤoneToNine_ЖDIGITↃParser.Instance);
        
        public static class _ʺx30ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._int._ʺx30ʺ> Instance { get; } = from _ʺx30ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx30ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._int._ʺx30ʺ.Instance;
        }
        
        public static class _ⲤoneToNine_ЖDIGITↃParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._int._ⲤoneToNine_ЖDIGITↃ> Instance { get; } = from _ⲤoneToNine_ЖDIGITↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤoneToNine_ЖDIGITↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._int._ⲤoneToNine_ЖDIGITↃ(_ⲤoneToNine_ЖDIGITↃ_1);
        }
    }
    
}
