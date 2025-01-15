namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _STARⳆ1ЖunreservedParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._STARⳆ1Жunreserved> Instance { get; } = (_STARParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._STARⳆ1Жunreserved>(_1ЖunreservedParser.Instance);
        
        public static class _STARParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._STARⳆ1Жunreserved._STAR> Instance { get; } = from _STAR_1 in __GeneratedOdata.Parsers.Rules._STARParser.Instance
select new __GeneratedOdata.CstNodes.Inners._STARⳆ1Жunreserved._STAR(_STAR_1);
        }
        
        public static class _1ЖunreservedParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._STARⳆ1Жunreserved._1Жunreserved> Instance { get; } = from _unreserved_1 in __GeneratedOdata.Parsers.Rules._unreservedParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Inners._STARⳆ1Жunreserved._1Жunreserved(new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Rules._unreserved>(_unreserved_1));
        }
    }
    
}
