namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⰳx30ⲻ34Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34> Instance { get; } = (_30Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34>(_31Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34>(_32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34>(_33Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34>(_34Parser.Instance);
        
        public static class _30Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._30> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._30>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._30> Parse(IInput<char>? input)
                {
                    var _30 = CombinatorParsingV2.Parse.Char((char)0x30).Parse(input);
if (!_30.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._30)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._30.Instance, _30.Remainder);
                }
            }
        }
        
        public static class _31Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._31> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._31>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._31> Parse(IInput<char>? input)
                {
                    var _31 = CombinatorParsingV2.Parse.Char((char)0x31).Parse(input);
if (!_31.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._31)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._31.Instance, _31.Remainder);
                }
            }
        }
        
        public static class _32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._32> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._32>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._32> Parse(IInput<char>? input)
                {
                    var _32 = CombinatorParsingV2.Parse.Char((char)0x32).Parse(input);
if (!_32.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._32)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._32.Instance, _32.Remainder);
                }
            }
        }
        
        public static class _33Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._33> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._33>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._33> Parse(IInput<char>? input)
                {
                    var _33 = CombinatorParsingV2.Parse.Char((char)0x33).Parse(input);
if (!_33.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._33)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._33.Instance, _33.Remainder);
                }
            }
        }
        
        public static class _34Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._34> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._34>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._34> Parse(IInput<char>? input)
                {
                    var _34 = CombinatorParsingV2.Parse.Char((char)0x34).Parse(input);
if (!_34.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._34)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._Ⰳx30ⲻ34._34.Instance, _34.Remainder);
                }
            }
        }
    }
    
}
