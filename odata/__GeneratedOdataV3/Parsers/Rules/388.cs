namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _hostParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._host> Instance { get; } = (_IPⲻliteralParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._host>(_IPv4addressParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._host>(_regⲻnameParser.Instance);
        
        public static class _IPⲻliteralParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._host._IPⲻliteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._host._IPⲻliteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._host._IPⲻliteral> Parse(IInput<char>? input)
                {
                    var _IPⲻliteral_1 = __GeneratedOdataV3.Parsers.Rules._IPⲻliteralParser.Instance.Parse(input);
if (!_IPⲻliteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._host._IPⲻliteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._host._IPⲻliteral(_IPⲻliteral_1.Parsed), _IPⲻliteral_1.Remainder);
                }
            }
        }
        
        public static class _IPv4addressParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._host._IPv4address> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._host._IPv4address>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._host._IPv4address> Parse(IInput<char>? input)
                {
                    var _IPv4address_1 = __GeneratedOdataV3.Parsers.Rules._IPv4addressParser.Instance.Parse(input);
if (!_IPv4address_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._host._IPv4address)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._host._IPv4address(_IPv4address_1.Parsed), _IPv4address_1.Remainder);
                }
            }
        }
        
        public static class _regⲻnameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._host._regⲻname> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._host._regⲻname>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._host._regⲻname> Parse(IInput<char>? input)
                {
                    var _regⲻname_1 = __GeneratedOdataV3.Parsers.Rules._regⲻnameParser.Instance.Parse(input);
if (!_regⲻname_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._host._regⲻname)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._host._regⲻname(_regⲻname_1.Parsed), _regⲻname_1.Remainder);
                }
            }
        }
    }
    
}
