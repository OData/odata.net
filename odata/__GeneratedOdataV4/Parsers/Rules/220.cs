namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _intParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._int> Instance { get; } = (_ʺx30ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._int>(_ⲤoneToNine_ЖDIGITↃParser.Instance);
        
        public static class _ʺx30ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._int._ʺx30ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._int._ʺx30ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._int._ʺx30ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx30ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx30ʺParser.Instance.Parse(input);
if (!_ʺx30ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._int._ʺx30ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._int._ʺx30ʺ.Instance, _ʺx30ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ⲤoneToNine_ЖDIGITↃParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._int._ⲤoneToNine_ЖDIGITↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._int._ⲤoneToNine_ЖDIGITↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._int._ⲤoneToNine_ЖDIGITↃ> Parse(IInput<char>? input)
                {
                    var _ⲤoneToNine_ЖDIGITↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤoneToNine_ЖDIGITↃParser.Instance.Parse(input);
if (!_ⲤoneToNine_ЖDIGITↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._int._ⲤoneToNine_ЖDIGITↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._int._ⲤoneToNine_ЖDIGITↃ(_ⲤoneToNine_ЖDIGITↃ_1.Parsed), _ⲤoneToNine_ЖDIGITↃ_1.Remainder);
                }
            }
        }
    }
    
}
