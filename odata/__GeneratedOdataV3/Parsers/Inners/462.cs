namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _base64b16Ⳇbase64b8Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8> Instance { get; } = (_base64b16Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8>(_base64b8Parser.Instance);
        
        public static class _base64b16Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b16> Instance { get; } = from _base64b16_1 in __GeneratedOdataV3.Parsers.Rules._base64b16Parser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b16(_base64b16_1);
        }
        
        public static class _base64b8Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b8> Instance { get; } = from _base64b8_1 in __GeneratedOdataV3.Parsers.Rules._base64b8Parser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._base64b16Ⳇbase64b8._base64b8(_base64b8_1);
        }
    }
    
}
