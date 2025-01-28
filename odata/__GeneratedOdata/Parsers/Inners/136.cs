namespace __GeneratedOdata.Parsers.Inners
{
    using __GeneratedOdata.CstNodes.Inners;
    using CombinatorParsingV2;
    
    public static class _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ> Instance { get; } = (_ʺx24x66x69x6Cx74x65x72ʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ>(_ʺx66x69x6Cx74x65x72ʺParser.Instance);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ> Instance { get; } = _ʺx24x66x69x6Cx74x65x72ʺParser.Instance;

        public static class _ʺx24x66x69x6Cx74x65x72ʺParser
        {
            /*public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ> Instance { get; } = from _ʺx24x66x69x6Cx74x65x72ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x66x69x6Cx74x65x72ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ(_ʺx24x66x69x6Cx74x65x72ʺ_1);
            */
            //// PERF
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ>
            {
                public IOutput<char, _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x66x69x6Cx74x65x72ʺ_1 = __GeneratedOdata.Parsers.Inners._ʺx24x66x69x6Cx74x65x72ʺParser.Instance.Parse(input);
                    return Output.Create(
                        true,
                        new __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ(_ʺx24x66x69x6Cx74x65x72ʺ_1.Parsed),
                        _ʺx24x66x69x6Cx74x65x72ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx66x69x6Cx74x65x72ʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ> Instance { get; } = from _ʺx66x69x6Cx74x65x72ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx66x69x6Cx74x65x72ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ(_ʺx66x69x6Cx74x65x72ʺ_1);
        }
    }
    
}
