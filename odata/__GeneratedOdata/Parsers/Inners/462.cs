namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _base64b16Ⳇbase64b8Parser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._base64b16Ⳇbase64b8> Instance { get; } = (_base64b16Parser.Instance).Or<__GeneratedOdata.CstNodes.Inners._base64b16Ⳇbase64b8>(_base64b8Parser.Instance);
        
        public static class _base64b16Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._base64b16Ⳇbase64b8._base64b16> Instance { get; } = from _base64b16_1 in __GeneratedOdata.Parsers.Rules._base64b16Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._base64b16Ⳇbase64b8._base64b16(_base64b16_1);
        }
        
        public static class _base64b8Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._base64b16Ⳇbase64b8._base64b8> Instance { get; } = from _base64b8_1 in __GeneratedOdata.Parsers.Rules._base64b8Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._base64b16Ⳇbase64b8._base64b8(_base64b8_1);
        }
    }
    
}
