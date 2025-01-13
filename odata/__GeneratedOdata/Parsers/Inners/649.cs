namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _unreservedⳆsubⲻdelimsⳆʺx3AʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ> Instance { get; } = (_unreservedParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ>(_subⲻdelimsParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ>(_ʺx3AʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved> Instance { get; } = from _unreserved_1 in __GeneratedOdata.Parsers.Rules._unreservedParser.Instance
select new __GeneratedOdata.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved(_unreserved_1);
        }
        
        public static class _subⲻdelimsParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims> Instance { get; } = from _subⲻdelims_1 in __GeneratedOdata.Parsers.Rules._subⲻdelimsParser.Instance
select new __GeneratedOdata.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims(_subⲻdelims_1);
        }
        
        public static class _ʺx3AʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ(_ʺx3Aʺ_1);
        }
    }
    
}
