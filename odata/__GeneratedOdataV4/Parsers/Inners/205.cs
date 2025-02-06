namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ> Instance { get; } = (_ʺx24x73x65x6Cx65x63x74ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ>(_ʺx73x65x6Cx65x63x74ʺParser.Instance);
        
        public static class _ʺx24x73x65x6Cx65x63x74ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx24x73x65x6Cx65x63x74ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx24x73x65x6Cx65x63x74ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx24x73x65x6Cx65x63x74ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x73x65x6Cx65x63x74ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x73x65x6Cx65x63x74ʺParser.Instance.Parse(input);
if (!_ʺx24x73x65x6Cx65x63x74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx24x73x65x6Cx65x63x74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx24x73x65x6Cx65x63x74ʺ.Instance, _ʺx24x73x65x6Cx65x63x74ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx73x65x6Cx65x63x74ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx73x65x6Cx65x63x74ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx73x65x6Cx65x63x74ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx73x65x6Cx65x63x74ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx73x65x6Cx65x63x74ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx73x65x6Cx65x63x74ʺParser.Instance.Parse(input);
if (!_ʺx73x65x6Cx65x63x74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx73x65x6Cx65x63x74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺ._ʺx73x65x6Cx65x63x74ʺ.Instance, _ʺx73x65x6Cx65x63x74ʺ_1.Remainder);
                }
            }
        }
    }
    
}
