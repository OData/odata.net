namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SPⳆHTABParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTAB> Instance { get; } = (_SPParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTAB>(_HTABParser.Instance);
        
        public static class _SPParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTAB._SP> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTAB._SP>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTAB._SP> Parse(IInput<char>? input)
                {
                    var _SP_1 = __GeneratedOdataV3.Parsers.Rules._SPParser.Instance.Parse(input);
if (!_SP_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SPⳆHTAB._SP)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTAB._SP.Instance, _SP_1.Remainder);
                }
            }
        }
        
        public static class _HTABParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTAB._HTAB> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTAB._HTAB>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTAB._HTAB> Parse(IInput<char>? input)
                {
                    var _HTAB_1 = __GeneratedOdataV3.Parsers.Rules._HTABParser.Instance.Parse(input);
if (!_HTAB_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SPⳆHTAB._HTAB)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTAB._HTAB.Instance, _HTAB_1.Remainder);
                }
            }
        }
    }
    
}
