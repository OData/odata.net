namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _IRIⲻinⲻqueryParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._IRIⲻinⲻquery> Instance { get; } = from _qcharⲻnoⲻAMP_1 in __GeneratedOdata.Parsers.Rules._qcharⲻnoⲻAMPParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Rules._IRIⲻinⲻquery(new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMP>(_qcharⲻnoⲻAMP_1));
    }
    
}
