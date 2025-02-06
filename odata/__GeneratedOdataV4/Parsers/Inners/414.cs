namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤoneToNine_ЖDIGITↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤoneToNine_ЖDIGITↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤoneToNine_ЖDIGITↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤoneToNine_ЖDIGITↃ> Parse(IInput<char>? input)
            {
                var _oneToNine_ЖDIGIT_1 = __GeneratedOdataV4.Parsers.Inners._oneToNine_ЖDIGITParser.Instance.Parse(input);
if (!_oneToNine_ЖDIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤoneToNine_ЖDIGITↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤoneToNine_ЖDIGITↃ(_oneToNine_ЖDIGIT_1.Parsed), _oneToNine_ЖDIGIT_1.Remainder);
            }
        }
    }
    
}
