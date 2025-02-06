namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSEParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE> Instance { get; } = (_ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Parser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE>(_count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡Parser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE>(_OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSEParser.Instance);
        
        public static class _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡> Parse(IInput<char>? input)
                {
                    var _ref_1 = __GeneratedOdataV4.Parsers.Rules._refParser.Instance.Parse(input);
if (!_ref_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡)!, input);
}

var _OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE_1 = __GeneratedOdataV4.Parsers.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSEParser.Instance.Optional().Parse(_ref_1.Remainder);
if (!_OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡(_ref_1.Parsed, _OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE_1.Parsed.GetOrElse(null)), _OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE_1.Remainder);
                }
            }
        }
        
        public static class _count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡> Parse(IInput<char>? input)
                {
                    var _count_1 = __GeneratedOdataV4.Parsers.Rules._countParser.Instance.Parse(input);
if (!_count_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡)!, input);
}

var _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1 = __GeneratedOdataV4.Parsers.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSEParser.Instance.Optional().Parse(_count_1.Remainder);
if (!_OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡(_count_1.Parsed, _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1.Parsed.GetOrElse(null)), _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1.Remainder);
                }
            }
        }
        
        public static class _OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSEParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE> Parse(IInput<char>? input)
                {
                    var _OPEN_1 = __GeneratedOdataV4.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE)!, input);
}

var _expandOption_1 = __GeneratedOdataV4.Parsers.Rules._expandOptionParser.Instance.Parse(_OPEN_1.Remainder);
if (!_expandOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE)!, input);
}

var _ⲤSEMI_expandOptionↃ_1 = Inners._ⲤSEMI_expandOptionↃParser.Instance.Many().Parse(_expandOption_1.Remainder);
if (!_ⲤSEMI_expandOptionↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE)!, input);
}

var _CLOSE_1 = __GeneratedOdataV4.Parsers.Rules._CLOSEParser.Instance.Parse(_ⲤSEMI_expandOptionↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE(_OPEN_1.Parsed, _expandOption_1.Parsed, _ⲤSEMI_expandOptionↃ_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
                }
            }
        }
    }
    
}
