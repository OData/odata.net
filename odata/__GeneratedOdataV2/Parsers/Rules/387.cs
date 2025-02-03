namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _userinfoParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._userinfo> Instance { get; } = from _ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ_1 in Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._userinfo(_ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ_1);
    }
    
}
