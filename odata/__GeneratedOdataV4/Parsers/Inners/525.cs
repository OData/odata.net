namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃ> Parse(IInput<char>? input)
            {
                var _ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT_1 = __GeneratedOdataV4.Parsers.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITParser.Instance.Parse(input);
if (!_ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃ(_ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT_1.Parsed), _ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT_1.Remainder);
            }
        }
    }
    
}
