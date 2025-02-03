namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _ls32Parser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._ls32> Instance { get; } = (_Ⲥh16_ʺx3Aʺ_h16ↃParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._ls32>(_IPv4addressParser.Instance);
        
        public static class _Ⲥh16_ʺx3Aʺ_h16ↃParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._ls32._Ⲥh16_ʺx3Aʺ_h16Ↄ> Instance { get; } = from _Ⲥh16_ʺx3Aʺ_h16Ↄ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥh16_ʺx3Aʺ_h16ↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._ls32._Ⲥh16_ʺx3Aʺ_h16Ↄ(_Ⲥh16_ʺx3Aʺ_h16Ↄ_1);
        }
        
        public static class _IPv4addressParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._ls32._IPv4address> Instance { get; } = from _IPv4address_1 in __GeneratedOdataV2.Parsers.Rules._IPv4addressParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._ls32._IPv4address(_IPv4address_1);
        }
    }
    
}
