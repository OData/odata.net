namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _repeatParser
    {
        public static Parser<__Generated.CstNodes.Rules._repeat> Instance { get; } = (_1ЖDIGITParser.Instance).Or<__Generated.CstNodes.Rules._repeat>(_ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃParser.Instance);
        
        public static class _1ЖDIGITParser
        {
            public static Parser<__Generated.CstNodes.Rules._repeat._1ЖDIGIT> Instance { get; } = from _DIGIT_1 in __GeneratedTest.Parsers.Rules._DIGITParser.Instance.Many()
select new __Generated.CstNodes.Rules._repeat._1ЖDIGIT(_DIGIT_1);
        }
        
        public static class _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃParser
        {
            public static Parser<__Generated.CstNodes.Rules._repeat._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ> Instance { get; } = from _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1 in __GeneratedTest.Parsers.Inners._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃParser.Instance
select new __Generated.CstNodes.Rules._repeat._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ(_ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1);
        }
    }
    
}
