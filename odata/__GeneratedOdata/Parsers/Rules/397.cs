namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _regⲻnameParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._regⲻname> Instance { get; } = from _ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._regⲻname(_ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ_1);
    }
    
}
