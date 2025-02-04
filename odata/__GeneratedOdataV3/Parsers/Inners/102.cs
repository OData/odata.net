namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ> Instance { get; } = (_ʺx24x65x78x70x61x6Ex64ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ>(_ʺx65x78x70x61x6Ex64ʺParser.Instance);
        
        public static class _ʺx24x65x78x70x61x6Ex64ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx24x65x78x70x61x6Ex64ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx24x65x78x70x61x6Ex64ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx24x65x78x70x61x6Ex64ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x65x78x70x61x6Ex64ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x65x78x70x61x6Ex64ʺParser.Instance.Parse(input);
if (!_ʺx24x65x78x70x61x6Ex64ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx24x65x78x70x61x6Ex64ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx24x65x78x70x61x6Ex64ʺ(_ʺx24x65x78x70x61x6Ex64ʺ_1.Parsed), _ʺx24x65x78x70x61x6Ex64ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx65x78x70x61x6Ex64ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx65x78x70x61x6Ex64ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx65x78x70x61x6Ex64ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx65x78x70x61x6Ex64ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx65x78x70x61x6Ex64ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx65x78x70x61x6Ex64ʺParser.Instance.Parse(input);
if (!_ʺx65x78x70x61x6Ex64ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx65x78x70x61x6Ex64ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx65x78x70x61x6Ex64ʺ(_ʺx65x78x70x61x6Ex64ʺ_1.Parsed), _ʺx65x78x70x61x6Ex64ʺ_1.Remainder);
                }
            }
        }
    }
    
}
