namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _requestⲻidParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._requestⲻid> Instance { get; } = from _unreserved_1 in __GeneratedOdata.Parsers.Rules._unreservedParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Rules._requestⲻid(new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Rules._unreserved>(_unreserved_1));
    }
    
}
