namespace __Generated.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃParser
    {
        public static IParser<char, __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ> Instance { get; } = (_1ЖⲤʺx2Eʺ_1ЖBITↃParser.Instance).Or<char, __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ>(_Ⲥʺx2Dʺ_1ЖBITↃParser.Instance);
        
        public static class _1ЖⲤʺx2Eʺ_1ЖBITↃParser
        {
            public static IParser<char, __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ._1ЖⲤʺx2Eʺ_1ЖBITↃ> Instance { get; } = from _Ⲥʺx2Eʺ_1ЖBITↃ_1 in __Generated.Parsers.Inners._Ⲥʺx2Eʺ_1ЖBITↃParser.Instance.Repeat(1, null)
select new __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ._1ЖⲤʺx2Eʺ_1ЖBITↃ(new __Generated.CstNodes.Inners.HelperRangedAtLeast1<__Generated.CstNodes.Inners._Ⲥʺx2Eʺ_1ЖBITↃ>(_Ⲥʺx2Eʺ_1ЖBITↃ_1));
        }
        
        public static class _Ⲥʺx2Dʺ_1ЖBITↃParser
        {
            public static IParser<char, __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ._Ⲥʺx2Dʺ_1ЖBITↃ> Instance { get; } = from _Ⲥʺx2Dʺ_1ЖBITↃ_1 in __Generated.Parsers.Inners._Ⲥʺx2Dʺ_1ЖBITↃParser.Instance
select new __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ._Ⲥʺx2Dʺ_1ЖBITↃ(_Ⲥʺx2Dʺ_1ЖBITↃ_1);
        }
    }
    
}
