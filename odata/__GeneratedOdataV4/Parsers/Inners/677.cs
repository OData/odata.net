namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _VCHARⳆobsⲻtextParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._VCHARⳆobsⲻtext> Instance { get; } = (_VCHARParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._VCHARⳆobsⲻtext>(_obsⲻtextParser.Instance);
        
        public static class _VCHARParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._VCHARⳆobsⲻtext._VCHAR> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._VCHARⳆobsⲻtext._VCHAR>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._VCHARⳆobsⲻtext._VCHAR> Parse(IInput<char>? input)
                {
                    var _VCHAR_1 = __GeneratedOdataV4.Parsers.Rules._VCHARParser.Instance.Parse(input);
if (!_VCHAR_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._VCHARⳆobsⲻtext._VCHAR)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._VCHARⳆobsⲻtext._VCHAR(_VCHAR_1.Parsed), _VCHAR_1.Remainder);
                }
            }
        }
        
        public static class _obsⲻtextParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._VCHARⳆobsⲻtext._obsⲻtext> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._VCHARⳆobsⲻtext._obsⲻtext>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._VCHARⳆobsⲻtext._obsⲻtext> Parse(IInput<char>? input)
                {
                    var _obsⲻtext_1 = __GeneratedOdataV4.Parsers.Rules._obsⲻtextParser.Instance.Parse(input);
if (!_obsⲻtext_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._VCHARⳆobsⲻtext._obsⲻtext)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._VCHARⳆobsⲻtext._obsⲻtext(_obsⲻtext_1.Parsed), _obsⲻtext_1.Remainder);
                }
            }
        }
    }
    
}
