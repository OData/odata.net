namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ> Instance { get; } = (_unreservedParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ>(_pctⲻencodedParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ>(_subⲻdelimsParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ>(_ʺx3AʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved> Instance { get; } = from _unreserved_1 in __GeneratedOdata.Parsers.Rules._unreservedParser.Instance
select new __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved(_unreserved_1);
        }
        
        public static class _pctⲻencodedParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._pctⲻencoded> Instance { get; } = from _pctⲻencoded_1 in __GeneratedOdata.Parsers.Rules._pctⲻencodedParser.Instance
select new __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._pctⲻencoded(_pctⲻencoded_1);
        }
        
        public static class _subⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims> Instance { get; } = from _subⲻdelims_1 in __GeneratedOdata.Parsers.Rules._subⲻdelimsParser.Instance
select new __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims(_subⲻdelims_1);
        }
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ(_ʺx3Aʺ_1);
        }
    }
    
}
