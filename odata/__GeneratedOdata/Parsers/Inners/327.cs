namespace __GeneratedOdata.Parsers.Inners
{
    using __GeneratedOdata.CstNodes.Inners;
    using CombinatorParsingV2;
    
    public static class _ʺx65x71ʺParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx65x71ʺ> Instance { get; } = from _x65_1 in __GeneratedOdata.Parsers.Inners._x65Parser.Instance
from _x71_1 in __GeneratedOdata.Parsers.Inners._x71Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx65x71ʺ(_x65_1, _x71_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx65x71ʺ> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx65x71ʺ>
        {
            public IOutput<char, _ʺx65x71ʺ> Parse(IInput<char>? input)
            {
                var _x65_1 = __GeneratedOdata.Parsers.Inners._x65Parser.Instance.Parse(input);
                if (!_x65_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_ʺx65x71ʺ)!,
                        input);
                }

                var _x71_1 = __GeneratedOdata.Parsers.Inners._x71Parser.Instance.Parse(_x65_1.Remainder);
                if (!_x71_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_ʺx65x71ʺ)!,
                        input);
                }

                return Output.Create(
                    true,
                    Instance,
                    _x71_1.Remainder);
            }

            private static __GeneratedOdata.CstNodes.Inners._ʺx65x71ʺ Instance { get; } = new __GeneratedOdata.CstNodes.Inners._ʺx65x71ʺ(_x65.Instance, _x71.Instance);
        }
    }
    
}
