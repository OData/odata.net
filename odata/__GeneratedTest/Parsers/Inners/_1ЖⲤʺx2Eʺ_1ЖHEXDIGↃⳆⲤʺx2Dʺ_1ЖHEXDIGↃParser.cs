namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃParser
    {
        public static Parser<__GeneratedTest.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ> Instance { get; } = (_1ЖⲤʺx2Eʺ_1ЖHEXDIGↃParser.Instance).Or<__GeneratedTest.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ>(_Ⲥʺx2Dʺ_1ЖHEXDIGↃParser.Instance);
        
        public static class _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃParser
        {
            public static Parser<__GeneratedTest.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃ> Instance { get; } = from _Ⲥʺx2Eʺ_1ЖHEXDIGↃ_1 in __GeneratedTest.Parsers.Inners._Ⲥʺx2Eʺ_1ЖHEXDIGↃParser.Instance.Many()
select new __GeneratedTest.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃ(_Ⲥʺx2Eʺ_1ЖHEXDIGↃ_1);
        }
        
        public static class _Ⲥʺx2Dʺ_1ЖHEXDIGↃParser
        {
            public static Parser<__GeneratedTest.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._Ⲥʺx2Dʺ_1ЖHEXDIGↃ> Instance { get; } = from _Ⲥʺx2Dʺ_1ЖHEXDIGↃ_1 in __GeneratedTest.Parsers.Inners._Ⲥʺx2Dʺ_1ЖHEXDIGↃParser.Instance
select new __GeneratedTest.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._Ⲥʺx2Dʺ_1ЖHEXDIGↃ(_Ⲥʺx2Dʺ_1ЖHEXDIGↃ_1);
        }
    }
    
}
