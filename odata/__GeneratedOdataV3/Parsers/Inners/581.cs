namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _termNameⳆSTARParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR> Instance { get; } = (_termNameParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR>(_STARParser.Instance);
        
        public static class _termNameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR._termName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR._termName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR._termName> Parse(IInput<char>? input)
                {
                    var _termName_1 = __GeneratedOdataV3.Parsers.Rules._termNameParser.Instance.Parse(input);
if (!_termName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR._termName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR._termName(_termName_1.Parsed), _termName_1.Remainder);
                }
            }
        }
        
        public static class _STARParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR._STAR> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR._STAR>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR._STAR> Parse(IInput<char>? input)
                {
                    var _STAR_1 = __GeneratedOdataV3.Parsers.Rules._STARParser.Instance.Parse(input);
if (!_STAR_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR._STAR)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._termNameⳆSTAR._STAR(_STAR_1.Parsed), _STAR_1.Remainder);
                }
            }
        }
    }
    
}
