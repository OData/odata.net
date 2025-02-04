namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ> Instance { get; } = (_ʺx24x66x69x6Cx74x65x72ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ>(_ʺx66x69x6Cx74x65x72ʺParser.Instance);
        
        public static class _ʺx24x66x69x6Cx74x65x72ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x66x69x6Cx74x65x72ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x66x69x6Cx74x65x72ʺParser.Instance.Parse(input);
if (!_ʺx24x66x69x6Cx74x65x72ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ.Instance, _ʺx24x66x69x6Cx74x65x72ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx66x69x6Cx74x65x72ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx66x69x6Cx74x65x72ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx66x69x6Cx74x65x72ʺParser.Instance.Parse(input);
if (!_ʺx66x69x6Cx74x65x72ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ.Instance, _ʺx66x69x6Cx74x65x72ʺ_1.Remainder);
                }
            }
        }
    }
    
}
