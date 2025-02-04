namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ> Instance { get; } = (_ʺx24x6Fx72x64x65x72x62x79ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ>(_ʺx6Fx72x64x65x72x62x79ʺParser.Instance);
        
        public static class _ʺx24x6Fx72x64x65x72x62x79ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx24x6Fx72x64x65x72x62x79ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx24x6Fx72x64x65x72x62x79ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx24x6Fx72x64x65x72x62x79ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x6Fx72x64x65x72x62x79ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x6Fx72x64x65x72x62x79ʺParser.Instance.Parse(input);
if (!_ʺx24x6Fx72x64x65x72x62x79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx24x6Fx72x64x65x72x62x79ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx24x6Fx72x64x65x72x62x79ʺ(_ʺx24x6Fx72x64x65x72x62x79ʺ_1.Parsed), _ʺx24x6Fx72x64x65x72x62x79ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx6Fx72x64x65x72x62x79ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx6Fx72x64x65x72x62x79ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx6Fx72x64x65x72x62x79ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx6Fx72x64x65x72x62x79ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx6Fx72x64x65x72x62x79ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx6Fx72x64x65x72x62x79ʺParser.Instance.Parse(input);
if (!_ʺx6Fx72x64x65x72x62x79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx6Fx72x64x65x72x62x79ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺ._ʺx6Fx72x64x65x72x62x79ʺ(_ʺx6Fx72x64x65x72x62x79ʺ_1.Parsed), _ʺx6Fx72x64x65x72x62x79ʺ_1.Remainder);
                }
            }
        }
    }
    
}
