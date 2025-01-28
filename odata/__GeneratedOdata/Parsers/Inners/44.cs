namespace __GeneratedOdata.Parsers.Inners
{
    using __GeneratedOdata.CstNodes.Inners;
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx2Fʺ_keyPathLiteralↃParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ> Instance { get; } = from _ʺx2Fʺ_keyPathLiteral_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_keyPathLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ(_ʺx2Fʺ_keyPathLiteral_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ>
        {
            public IOutput<char, _Ⲥʺx2Fʺ_keyPathLiteralↃ> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_keyPathLiteral_1 = __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_keyPathLiteralParser.Instance.Parse(input);
                if (!_ʺx2Fʺ_keyPathLiteral_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_Ⲥʺx2Fʺ_keyPathLiteralↃ)!,
                        _ʺx2Fʺ_keyPathLiteral_1.Remainder);
                }

                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ(_ʺx2Fʺ_keyPathLiteral_1.Parsed),
                    _ʺx2Fʺ_keyPathLiteral_1.Remainder);
            }
        }
    }
    
}
