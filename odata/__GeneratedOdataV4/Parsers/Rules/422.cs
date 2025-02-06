namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _HEXDIGParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._HEXDIG> Instance { get; } = (_DIGITParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._HEXDIG>(_AⲻtoⲻFParser.Instance);
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._HEXDIG._DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._HEXDIG._DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._HEXDIG._DIGIT> Parse(IInput<char>? input)
                {
                    var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._HEXDIG._DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._HEXDIG._DIGIT(_DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
        
        public static class _AⲻtoⲻFParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._HEXDIG._AⲻtoⲻF> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._HEXDIG._AⲻtoⲻF>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._HEXDIG._AⲻtoⲻF> Parse(IInput<char>? input)
                {
                    var _AⲻtoⲻF_1 = __GeneratedOdataV4.Parsers.Rules._AⲻtoⲻFParser.Instance.Parse(input);
if (!_AⲻtoⲻF_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._HEXDIG._AⲻtoⲻF)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._HEXDIG._AⲻtoⲻF(_AⲻtoⲻF_1.Parsed), _AⲻtoⲻF_1.Remainder);
                }
            }
        }
    }
    
}
