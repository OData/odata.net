namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute> Instance { get; } = (_ʺx5AʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute>(_SIGN_hour_ʺx3Aʺ_minuteParser.Instance);
        
        public static class _ʺx5AʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._ʺx5Aʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._ʺx5Aʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._ʺx5Aʺ> Parse(IInput<char>? input)
                {
                    var _ʺx5Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx5AʺParser.Instance.Parse(input);
if (!_ʺx5Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._ʺx5Aʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._ʺx5Aʺ.Instance, _ʺx5Aʺ_1.Remainder);
                }
            }
        }
        
        public static class _SIGN_hour_ʺx3Aʺ_minuteParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._SIGN_hour_ʺx3Aʺ_minute> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._SIGN_hour_ʺx3Aʺ_minute>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._SIGN_hour_ʺx3Aʺ_minute> Parse(IInput<char>? input)
                {
                    var _SIGN_1 = __GeneratedOdataV3.Parsers.Rules._SIGNParser.Instance.Parse(input);
if (!_SIGN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._SIGN_hour_ʺx3Aʺ_minute)!, input);
}

var _hour_1 = __GeneratedOdataV3.Parsers.Rules._hourParser.Instance.Parse(_SIGN_1.Remainder);
if (!_hour_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._SIGN_hour_ʺx3Aʺ_minute)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_hour_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._SIGN_hour_ʺx3Aʺ_minute)!, input);
}

var _minute_1 = __GeneratedOdataV3.Parsers.Rules._minuteParser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_minute_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._SIGN_hour_ʺx3Aʺ_minute)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._SIGN_hour_ʺx3Aʺ_minute(_SIGN_1.Parsed, _hour_1.Parsed, _ʺx3Aʺ_1.Parsed, _minute_1.Parsed), _minute_1.Remainder);
                }
            }
        }
    }
    
}
