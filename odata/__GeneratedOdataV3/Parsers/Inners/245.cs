namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ> Instance { get; } = (_ʺx2Fx24x65x6Ex74x69x74x79ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ>(_ʺx2Fx24x64x65x6Cx74x61ʺParser.Instance);
        
        public static class _ʺx2Fx24x65x6Ex74x69x74x79ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x65x6Ex74x69x74x79ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x65x6Ex74x69x74x79ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x65x6Ex74x69x74x79ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Fx24x65x6Ex74x69x74x79ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺParser.Instance.Parse(input);
if (!_ʺx2Fx24x65x6Ex74x69x74x79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x65x6Ex74x69x74x79ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x65x6Ex74x69x74x79ʺ.Instance, _ʺx2Fx24x65x6Ex74x69x74x79ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2Fx24x64x65x6Cx74x61ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x64x65x6Cx74x61ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x64x65x6Cx74x61ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x64x65x6Cx74x61ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Fx24x64x65x6Cx74x61ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fx24x64x65x6Cx74x61ʺParser.Instance.Parse(input);
if (!_ʺx2Fx24x64x65x6Cx74x61ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x64x65x6Cx74x61ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ._ʺx2Fx24x64x65x6Cx74x61ʺ.Instance, _ʺx2Fx24x64x65x6Cx74x61ʺ_1.Remainder);
                }
            }
        }
    }
    
}
