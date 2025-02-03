namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⰳx30ⲻ34Parser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34> Instance { get; } = (_30Parser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34>(_31Parser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34>(_32Parser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34>(_33Parser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34>(_34Parser.Instance);
        
        public static class _30Parser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34._30> Instance { get; } = from _30 in Parse.Char((char)0x30) select new __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34._30(__GeneratedOdataV2.CstNodes.Inners._3.Instance, __GeneratedOdataV2.CstNodes.Inners._0.Instance);
        }
        
        public static class _31Parser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34._31> Instance { get; } = from _31 in Parse.Char((char)0x31) select new __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34._31(__GeneratedOdataV2.CstNodes.Inners._3.Instance, __GeneratedOdataV2.CstNodes.Inners._1.Instance);
        }
        
        public static class _32Parser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34._32> Instance { get; } = from _32 in Parse.Char((char)0x32) select new __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34._32(__GeneratedOdataV2.CstNodes.Inners._3.Instance, __GeneratedOdataV2.CstNodes.Inners._2.Instance);
        }
        
        public static class _33Parser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34._33> Instance { get; } = from _33 in Parse.Char((char)0x33) select new __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34._33(__GeneratedOdataV2.CstNodes.Inners._3.Instance, __GeneratedOdataV2.CstNodes.Inners._3.Instance);
        }
        
        public static class _34Parser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34._34> Instance { get; } = from _34 in Parse.Char((char)0x34) select new __GeneratedOdataV2.CstNodes.Inners._Ⰳx30ⲻ34._34(__GeneratedOdataV2.CstNodes.Inners._3.Instance, __GeneratedOdataV2.CstNodes.Inners._4.Instance);
        }
    }
    
}
