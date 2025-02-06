namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx2Fʺ_keyPathLiteralↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_keyPathLiteral_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2Fʺ_keyPathLiteralParser.Instance.Parse(input);
if (!_ʺx2Fʺ_keyPathLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ(_ʺx2Fʺ_keyPathLiteral_1.Parsed), _ʺx2Fʺ_keyPathLiteral_1.Remainder);
            }
        }
    }
    
}
