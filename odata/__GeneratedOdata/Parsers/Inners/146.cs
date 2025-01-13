namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx61x73x63ʺⳆʺx64x65x73x63ʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ> Instance { get; } = (_ʺx61x73x63ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ>(_ʺx64x65x73x63ʺParser.Instance);
        
        public static class _ʺx61x73x63ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx61x73x63ʺ> Instance { get; } = from _ʺx61x73x63ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx61x73x63ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx61x73x63ʺ(_ʺx61x73x63ʺ_1);
        }
        
        public static class _ʺx64x65x73x63ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx64x65x73x63ʺ> Instance { get; } = from _ʺx64x65x73x63ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx64x65x73x63ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx64x65x73x63ʺ(_ʺx64x65x73x63ʺ_1);
        }
    }
    
}
