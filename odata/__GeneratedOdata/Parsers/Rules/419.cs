namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _IRIⲻinⲻqueryParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._IRIⲻinⲻquery> Instance { get; } = from _qcharⲻnoⲻAMP_1 in __GeneratedOdata.Parsers.Rules._qcharⲻnoⲻAMPParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._IRIⲻinⲻquery(_qcharⲻnoⲻAMP_1);
    }
    
}
