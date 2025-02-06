namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _ls32Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._ls32> Instance { get; } = (_Ⲥh16_ʺx3Aʺ_h16ↃParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._ls32>(_IPv4addressParser.Instance);
        
        public static class _Ⲥh16_ʺx3Aʺ_h16ↃParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._ls32._Ⲥh16_ʺx3Aʺ_h16Ↄ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._ls32._Ⲥh16_ʺx3Aʺ_h16Ↄ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._ls32._Ⲥh16_ʺx3Aʺ_h16Ↄ> Parse(IInput<char>? input)
                {
                    var _Ⲥh16_ʺx3Aʺ_h16Ↄ_1 = __GeneratedOdataV4.Parsers.Inners._Ⲥh16_ʺx3Aʺ_h16ↃParser.Instance.Parse(input);
if (!_Ⲥh16_ʺx3Aʺ_h16Ↄ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._ls32._Ⲥh16_ʺx3Aʺ_h16Ↄ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._ls32._Ⲥh16_ʺx3Aʺ_h16Ↄ(_Ⲥh16_ʺx3Aʺ_h16Ↄ_1.Parsed), _Ⲥh16_ʺx3Aʺ_h16Ↄ_1.Remainder);
                }
            }
        }
        
        public static class _IPv4addressParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._ls32._IPv4address> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._ls32._IPv4address>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._ls32._IPv4address> Parse(IInput<char>? input)
                {
                    var _IPv4address_1 = __GeneratedOdataV4.Parsers.Rules._IPv4addressParser.Instance.Parse(input);
if (!_IPv4address_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._ls32._IPv4address)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._ls32._IPv4address(_IPv4address_1.Parsed), _IPv4address_1.Remainder);
                }
            }
        }
    }
    
}
