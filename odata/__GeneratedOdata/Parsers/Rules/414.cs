namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR> Instance { get; } = (_unreservedParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>(_pctⲻencodedParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>(_otherⲻdelimsParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>(_ʺx3AʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>(_ʺx2FʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>(_ʺx3FʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>(_ʺx27ʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._unreserved> Instance { get; } = from _unreserved_1 in __GeneratedOdata.Parsers.Rules._unreservedParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._unreserved(_unreserved_1);
        }
        
        public static class _pctⲻencodedParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._pctⲻencoded> Instance { get; } = from _pctⲻencoded_1 in __GeneratedOdata.Parsers.Rules._pctⲻencodedParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._pctⲻencoded(_pctⲻencoded_1);
        }
        
        public static class _otherⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._otherⲻdelims> Instance { get; } = from _otherⲻdelims_1 in __GeneratedOdata.Parsers.Rules._otherⲻdelimsParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._otherⲻdelims(_otherⲻdelims_1);
        }
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Aʺ> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Aʺ(_ʺx3Aʺ_1);
        }
        
        public static class _ʺx2FʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx2Fʺ> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx2Fʺ(_ʺx2Fʺ_1);
        }
        
        public static class _ʺx3FʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Fʺ> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3FʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Fʺ(_ʺx3Fʺ_1);
        }
        
        public static class _ʺx27ʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx27ʺ> Instance { get; } = from _ʺx27ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx27ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx27ʺ(_ʺx27ʺ_1);
        }
    }
    
}
