namespace __GeneratedOdata.Parsers.Inners
{
    using __Generated.CstNodes.Inners;
    using __GeneratedOdata.CstNodes.Inners;
    using CombinatorParsingV2;
    
    public static class _ʺx24x66x69x6Cx74x65x72ʺParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺ> Instance { get; } = from _x24_1 in __GeneratedOdata.Parsers.Inners._x24Parser.Instance
from _x66_1 in __GeneratedOdata.Parsers.Inners._x66Parser.Instance
from _x69_1 in __GeneratedOdata.Parsers.Inners._x69Parser.Instance
from _x6C_1 in __GeneratedOdata.Parsers.Inners._x6CParser.Instance
from _x74_1 in __GeneratedOdata.Parsers.Inners._x74Parser.Instance
from _x65_1 in __GeneratedOdata.Parsers.Inners._x65Parser.Instance
from _x72_1 in __GeneratedOdata.Parsers.Inners._x72Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺ(_x24_1, _x66_1, _x69_1, _x6C_1, _x74_1, _x65_1, _x72_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺ> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺ>
        {
            public IOutput<char, _ʺx24x66x69x6Cx74x65x72ʺ> Parse(IInput<char>? input)
            {
                var _x24_1 = __GeneratedOdata.Parsers.Inners._x24Parser.Instance.Parse(input);
                var _x66_1 = __GeneratedOdata.Parsers.Inners._x66Parser.Instance.Parse(_x24_1.Remainder);
                var _x69_1 = __GeneratedOdata.Parsers.Inners._x69Parser.Instance.Parse(_x66_1.Remainder);
                var _x6C_1 = __GeneratedOdata.Parsers.Inners._x6CParser.Instance.Parse(_x69_1.Remainder);
                var _x74_1 = __GeneratedOdata.Parsers.Inners._x74Parser.Instance.Parse(_x6C_1.Remainder);
                var _x65_1 = __GeneratedOdata.Parsers.Inners._x65Parser.Instance.Parse(_x74_1.Remainder);
                var _x72_1 = __GeneratedOdata.Parsers.Inners._x72Parser.Instance.Parse(_x65_1.Remainder);
                return Output.Create(
                    true,
                    Instance,
                    _x72_1.Remainder);
            }
            private static __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺ Instance { get; } = new __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺ(_x24.Instance, _x66.Instance, _x69.Instance, _x6C.Instance, _x74.Instance, _x65.Instance, _x72.Instance);
        }
    }
    
}
