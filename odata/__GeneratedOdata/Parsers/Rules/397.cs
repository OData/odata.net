namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _regⲻnameParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._regⲻname> Instance { get; } = from _ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ_1 in Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._regⲻname(_ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ_1);
    }
    
}
