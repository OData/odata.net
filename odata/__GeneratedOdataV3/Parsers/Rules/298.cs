namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _yearParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._year> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._year>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._year> Parse(IInput<char>? input)
            {
                var _ʺx2Dʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2DʺParser.Instance.Optional().Parse(input);
if (!_ʺx2Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._year)!, input);
}

var _Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃParser.Instance.Parse(_ʺx2Dʺ_1.Remainder);
if (!_Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._year)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._year(_ʺx2Dʺ_1.Parsed.GetOrElse(null), _Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃ_1.Parsed), _Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃ_1.Remainder);
            }
        }
    }
    
}
