namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _unreservedⳆpctⲻencodedⳆsubⲻdelimsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims> Instance { get; } = (_unreservedParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims>(_pctⲻencodedParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims>(_subⲻdelimsParser.Instance);
        
        public static class _unreservedParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._unreserved> Instance { get; } = from _unreserved_1 in __GeneratedOdataV3.Parsers.Rules._unreservedParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._unreserved(_unreserved_1);
        }
        
        public static class _pctⲻencodedParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._pctⲻencoded> Instance { get; } = from _pctⲻencoded_1 in __GeneratedOdataV3.Parsers.Rules._pctⲻencodedParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._pctⲻencoded(_pctⲻencoded_1);
        }
        
        public static class _subⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._subⲻdelims> Instance { get; } = from _subⲻdelims_1 in __GeneratedOdataV3.Parsers.Rules._subⲻdelimsParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._subⲻdelims(_subⲻdelims_1);
        }
    }
    
}
