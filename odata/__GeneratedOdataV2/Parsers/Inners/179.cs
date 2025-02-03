namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _STARⳆ1ЖunreservedParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._STARⳆ1Жunreserved> Instance { get; } = (_STARParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._STARⳆ1Жunreserved>(_1ЖunreservedParser.Instance);
        
        public static class _STARParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._STARⳆ1Жunreserved._STAR> Instance { get; } = from _STAR_1 in __GeneratedOdataV2.Parsers.Rules._STARParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._STARⳆ1Жunreserved._STAR(_STAR_1);
        }
        
        public static class _1ЖunreservedParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._STARⳆ1Жunreserved._1Жunreserved> Instance { get; } = from _unreserved_1 in __GeneratedOdataV2.Parsers.Rules._unreservedParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Inners._STARⳆ1Жunreserved._1Жunreserved(new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._unreserved>(_unreserved_1));
        }
    }
    
}
