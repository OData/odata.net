namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _expandRefOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption> Instance { get; } = (_expandCountOptionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption>(_orderbyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption>(_skipParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption>(_topParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption>(_inlinecountParser.Instance);
        
        public static class _expandCountOptionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._expandCountOption> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._expandCountOption>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._expandCountOption> Parse(IInput<char>? input)
                {
                    var _expandCountOption_1 = __GeneratedOdataV3.Parsers.Rules._expandCountOptionParser.Instance.Parse(input);
if (!_expandCountOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._expandRefOption._expandCountOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._expandRefOption._expandCountOption(_expandCountOption_1.Parsed), _expandCountOption_1.Remainder);
                }
            }
        }
        
        public static class _orderbyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._orderby> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._orderby>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._orderby> Parse(IInput<char>? input)
                {
                    var _orderby_1 = __GeneratedOdataV3.Parsers.Rules._orderbyParser.Instance.Parse(input);
if (!_orderby_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._expandRefOption._orderby)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._expandRefOption._orderby(_orderby_1.Parsed), _orderby_1.Remainder);
                }
            }
        }
        
        public static class _skipParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._skip> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._skip>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._skip> Parse(IInput<char>? input)
                {
                    var _skip_1 = __GeneratedOdataV3.Parsers.Rules._skipParser.Instance.Parse(input);
if (!_skip_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._expandRefOption._skip)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._expandRefOption._skip(_skip_1.Parsed), _skip_1.Remainder);
                }
            }
        }
        
        public static class _topParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._top> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._top>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._top> Parse(IInput<char>? input)
                {
                    var _top_1 = __GeneratedOdataV3.Parsers.Rules._topParser.Instance.Parse(input);
if (!_top_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._expandRefOption._top)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._expandRefOption._top(_top_1.Parsed), _top_1.Remainder);
                }
            }
        }
        
        public static class _inlinecountParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._inlinecount> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._inlinecount>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._expandRefOption._inlinecount> Parse(IInput<char>? input)
                {
                    var _inlinecount_1 = __GeneratedOdataV3.Parsers.Rules._inlinecountParser.Instance.Parse(input);
if (!_inlinecount_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._expandRefOption._inlinecount)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._expandRefOption._inlinecount(_inlinecount_1.Parsed), _inlinecount_1.Remainder);
                }
            }
        }
    }
    
}
