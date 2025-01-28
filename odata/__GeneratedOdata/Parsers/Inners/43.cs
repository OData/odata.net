namespace __GeneratedOdata.Parsers.Inners
{
    using __GeneratedOdata.CstNodes.Inners;
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_keyPathLiteralParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _keyPathLiteral_1 in __GeneratedOdata.Parsers.Rules._keyPathLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral(_ʺx2Fʺ_1, _keyPathLiteral_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral>
        {
            public IOutput<char, _ʺx2Fʺ_keyPathLiteral> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);

                if (!_ʺx2Fʺ_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_ʺx2Fʺ_keyPathLiteral)!,
                        input);
                }

                var _keyPathLiteral_1 = __GeneratedOdata.Parsers.Rules._keyPathLiteralParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
                if (!_keyPathLiteral_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_ʺx2Fʺ_keyPathLiteral)!,
                        input);
                }

                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral(_ʺx2Fʺ_1.Parsed, _keyPathLiteral_1.Parsed),
                    _keyPathLiteral_1.Remainder);
            }
        }
    }
    
}
