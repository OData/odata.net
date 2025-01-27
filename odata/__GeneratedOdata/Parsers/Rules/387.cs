namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _userinfoParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._userinfo> Instance { get; } = from _ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ_1 in Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._userinfo(_ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ_1);
    }
    
}
