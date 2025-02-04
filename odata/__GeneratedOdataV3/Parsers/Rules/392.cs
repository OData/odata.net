namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _IPv6addressParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address> Instance { get; } = (_6Ⲥh16_ʺx3AʺↃ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡Ж6Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3AʺParser.Instance);
        
        public static class _6Ⲥh16_ʺx3AʺↃ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._6Ⲥh16_ʺx3AʺↃ_ls32> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._6Ⲥh16_ʺx3AʺↃ_ls32>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._6Ⲥh16_ʺx3AʺↃ_ls32> Parse(IInput<char>? input)
                {
                    var _Ⲥh16_ʺx3AʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(6, 6).Parse(input);
if (!_Ⲥh16_ʺx3AʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._6Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

var _ls32_1 = __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance.Parse(_Ⲥh16_ʺx3AʺↃ_1.Remainder);
if (!_ls32_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._6Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._IPv6address._6Ⲥh16_ʺx3AʺↃ_ls32(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly6<__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1.Parsed), _ls32_1.Parsed), _ls32_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32> Parse(IInput<char>? input)
                {
                    var _ʺx3Ax3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance.Parse(input);
if (!_ʺx3Ax3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

var _Ⲥh16_ʺx3AʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(5, 5).Parse(_ʺx3Ax3Aʺ_1.Remainder);
if (!_Ⲥh16_ʺx3AʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

var _ls32_1 = __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance.Parse(_Ⲥh16_ʺx3AʺↃ_1.Remainder);
if (!_ls32_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._IPv6address._ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32(_ʺx3Ax3Aʺ_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly5<__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1.Parsed), _ls32_1.Parsed), _ls32_1.Remainder);
                }
            }
        }
        
        public static class _꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32> Parse(IInput<char>? input)
                {
                    var _h16_1 = __GeneratedOdataV3.Parsers.Rules._h16Parser.Instance.Optional().Parse(input);
if (!_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

var _ʺx3Ax3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance.Parse(_h16_1.Remainder);
if (!_ʺx3Ax3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

var _Ⲥh16_ʺx3AʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(4, 4).Parse(_ʺx3Ax3Aʺ_1.Remainder);
if (!_Ⲥh16_ʺx3AʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

var _ls32_1 = __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance.Parse(_Ⲥh16_ʺx3AʺↃ_1.Remainder);
if (!_ls32_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32(_h16_1.Parsed.GetOrElse(null), _ʺx3Ax3Aʺ_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly4<__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1.Parsed), _ls32_1.Parsed), _ls32_1.Remainder);
                }
            }
        }
        
        public static class _꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32> Parse(IInput<char>? input)
                {
                    var _Ж1Ⲥh16_ʺx3AʺↃ_h16_1 = __GeneratedOdataV3.Parsers.Inners._Ж1Ⲥh16_ʺx3AʺↃ_h16Parser.Instance.Optional().Parse(input);
if (!_Ж1Ⲥh16_ʺx3AʺↃ_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

var _ʺx3Ax3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance.Parse(_Ж1Ⲥh16_ʺx3AʺↃ_h16_1.Remainder);
if (!_ʺx3Ax3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

var _Ⲥh16_ʺx3AʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(3, 3).Parse(_ʺx3Ax3Aʺ_1.Remainder);
if (!_Ⲥh16_ʺx3AʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

var _ls32_1 = __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance.Parse(_Ⲥh16_ʺx3AʺↃ_1.Remainder);
if (!_ls32_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32(_Ж1Ⲥh16_ʺx3AʺↃ_h16_1.Parsed.GetOrElse(null), _ʺx3Ax3Aʺ_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly3<__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1.Parsed), _ls32_1.Parsed), _ls32_1.Remainder);
                }
            }
        }
        
        public static class _꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32> Parse(IInput<char>? input)
                {
                    var _Ж2Ⲥh16_ʺx3AʺↃ_h16_1 = __GeneratedOdataV3.Parsers.Inners._Ж2Ⲥh16_ʺx3AʺↃ_h16Parser.Instance.Optional().Parse(input);
if (!_Ж2Ⲥh16_ʺx3AʺↃ_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

var _ʺx3Ax3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance.Parse(_Ж2Ⲥh16_ʺx3AʺↃ_h16_1.Remainder);
if (!_ʺx3Ax3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

var _Ⲥh16_ʺx3AʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(2, 2).Parse(_ʺx3Ax3Aʺ_1.Remainder);
if (!_Ⲥh16_ʺx3AʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

var _ls32_1 = __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance.Parse(_Ⲥh16_ʺx3AʺↃ_1.Remainder);
if (!_ls32_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32(_Ж2Ⲥh16_ʺx3AʺↃ_h16_1.Parsed.GetOrElse(null), _ʺx3Ax3Aʺ_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly2<__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1.Parsed), _ls32_1.Parsed), _ls32_1.Remainder);
                }
            }
        }
        
        public static class _꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32> Parse(IInput<char>? input)
                {
                    var _Ж3Ⲥh16_ʺx3AʺↃ_h16_1 = __GeneratedOdataV3.Parsers.Inners._Ж3Ⲥh16_ʺx3AʺↃ_h16Parser.Instance.Optional().Parse(input);
if (!_Ж3Ⲥh16_ʺx3AʺↃ_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32)!, input);
}

var _ʺx3Ax3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance.Parse(_Ж3Ⲥh16_ʺx3AʺↃ_h16_1.Remainder);
if (!_ʺx3Ax3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32)!, input);
}

var _h16_1 = __GeneratedOdataV3.Parsers.Rules._h16Parser.Instance.Parse(_ʺx3Ax3Aʺ_1.Remainder);
if (!_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_h16_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32)!, input);
}

var _ls32_1 = __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_ls32_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32(_Ж3Ⲥh16_ʺx3AʺↃ_h16_1.Parsed.GetOrElse(null), _ʺx3Ax3Aʺ_1.Parsed, _h16_1.Parsed, _ʺx3Aʺ_1.Parsed, _ls32_1.Parsed), _ls32_1.Remainder);
                }
            }
        }
        
        public static class _꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32> Parse(IInput<char>? input)
                {
                    var _Ж4Ⲥh16_ʺx3AʺↃ_h16_1 = __GeneratedOdataV3.Parsers.Inners._Ж4Ⲥh16_ʺx3AʺↃ_h16Parser.Instance.Optional().Parse(input);
if (!_Ж4Ⲥh16_ʺx3AʺↃ_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32)!, input);
}

var _ʺx3Ax3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance.Parse(_Ж4Ⲥh16_ʺx3AʺↃ_h16_1.Remainder);
if (!_ʺx3Ax3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32)!, input);
}

var _ls32_1 = __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance.Parse(_ʺx3Ax3Aʺ_1.Remainder);
if (!_ls32_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32(_Ж4Ⲥh16_ʺx3AʺↃ_h16_1.Parsed.GetOrElse(null), _ʺx3Ax3Aʺ_1.Parsed, _ls32_1.Parsed), _ls32_1.Remainder);
                }
            }
        }
        
        public static class _꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16> Parse(IInput<char>? input)
                {
                    var _Ж5Ⲥh16_ʺx3AʺↃ_h16_1 = __GeneratedOdataV3.Parsers.Inners._Ж5Ⲥh16_ʺx3AʺↃ_h16Parser.Instance.Optional().Parse(input);
if (!_Ж5Ⲥh16_ʺx3AʺↃ_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16)!, input);
}

var _ʺx3Ax3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance.Parse(_Ж5Ⲥh16_ʺx3AʺↃ_h16_1.Remainder);
if (!_ʺx3Ax3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16)!, input);
}

var _h16_1 = __GeneratedOdataV3.Parsers.Rules._h16Parser.Instance.Parse(_ʺx3Ax3Aʺ_1.Remainder);
if (!_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16(_Ж5Ⲥh16_ʺx3AʺↃ_h16_1.Parsed.GetOrElse(null), _ʺx3Ax3Aʺ_1.Parsed, _h16_1.Parsed), _h16_1.Remainder);
                }
            }
        }
        
        public static class _꘡Ж6Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3AʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж6Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж6Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж6Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ> Parse(IInput<char>? input)
                {
                    var _Ж6Ⲥh16_ʺx3AʺↃ_h16_1 = __GeneratedOdataV3.Parsers.Inners._Ж6Ⲥh16_ʺx3AʺↃ_h16Parser.Instance.Optional().Parse(input);
if (!_Ж6Ⲥh16_ʺx3AʺↃ_h16_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж6Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ)!, input);
}

var _ʺx3Ax3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance.Parse(_Ж6Ⲥh16_ʺx3AʺↃ_h16_1.Remainder);
if (!_ʺx3Ax3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж6Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж6Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ(_Ж6Ⲥh16_ʺx3AʺↃ_h16_1.Parsed.GetOrElse(null), _ʺx3Ax3Aʺ_1.Parsed), _ʺx3Ax3Aʺ_1.Remainder);
                }
            }
        }
    }
    
}
