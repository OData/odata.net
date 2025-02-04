namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ> Instance { get; } = (_ʺx24x6Cx65x76x65x6Cx73ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ>(_ʺx6Cx65x76x65x6Cx73ʺParser.Instance);
        
        public static class _ʺx24x6Cx65x76x65x6Cx73ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx24x6Cx65x76x65x6Cx73ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx24x6Cx65x76x65x6Cx73ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx24x6Cx65x76x65x6Cx73ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x6Cx65x76x65x6Cx73ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x6Cx65x76x65x6Cx73ʺParser.Instance.Parse(input);
if (!_ʺx24x6Cx65x76x65x6Cx73ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx24x6Cx65x76x65x6Cx73ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx24x6Cx65x76x65x6Cx73ʺ.Instance, _ʺx24x6Cx65x76x65x6Cx73ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx6Cx65x76x65x6Cx73ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx6Cx65x76x65x6Cx73ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx6Cx65x76x65x6Cx73ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx6Cx65x76x65x6Cx73ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx6Cx65x76x65x6Cx73ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx6Cx65x76x65x6Cx73ʺParser.Instance.Parse(input);
if (!_ʺx6Cx65x76x65x6Cx73ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx6Cx65x76x65x6Cx73ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺ._ʺx6Cx65x76x65x6Cx73ʺ.Instance, _ʺx6Cx65x76x65x6Cx73ʺ_1.Remainder);
                }
            }
        }
    }
    
}
