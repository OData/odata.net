namespace __Generated.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃParser
    {
        public static IParser<char, __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ> Instance { get; } = (_1ЖⲤʺx2Eʺ_1ЖDIGITↃParser.Instance).Or<char, __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ>(_Ⲥʺx2Dʺ_1ЖDIGITↃParser.Instance);
        
        public static class _1ЖⲤʺx2Eʺ_1ЖDIGITↃParser
        {
            public static IParser<char, __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ._1ЖⲤʺx2Eʺ_1ЖDIGITↃ> Instance { get; } = from _Ⲥʺx2Eʺ_1ЖDIGITↃ_1 in __Generated.Parsers.Inners._Ⲥʺx2Eʺ_1ЖDIGITↃParser.Instance.Repeat(1, null)
select new __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ._1ЖⲤʺx2Eʺ_1ЖDIGITↃ(new __Generated.CstNodes.Inners.HelperRangedAtLeast1<__Generated.CstNodes.Inners._Ⲥʺx2Eʺ_1ЖDIGITↃ>(_Ⲥʺx2Eʺ_1ЖDIGITↃ_1));
        }
        
        public static class _Ⲥʺx2Dʺ_1ЖDIGITↃParser
        {
            public static IParser<char, __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ._Ⲥʺx2Dʺ_1ЖDIGITↃ> Instance { get; } = from _Ⲥʺx2Dʺ_1ЖDIGITↃ_1 in __Generated.Parsers.Inners._Ⲥʺx2Dʺ_1ЖDIGITↃParser.Instance
select new __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ._Ⲥʺx2Dʺ_1ЖDIGITↃ(_Ⲥʺx2Dʺ_1ЖDIGITↃ_1);
        }
    }
    
}
