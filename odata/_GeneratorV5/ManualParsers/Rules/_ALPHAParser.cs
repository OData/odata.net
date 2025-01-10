namespace _GeneratorV5.ManualParsers.Rules
{
    using Sprache;

    public static class _ALPHAParser
    {
        public static Parser<__Generated.CstNodes.Rules._ALPHA> Instance { get; } =
            Inners._Ⰳx41ⲻ5A
            .Or<__Generated.CstNodes.Rules._ALPHA>(Inners._Ⰳx61ⲻ7A);

        private static class Inners
        {
            public static Parser<__Generated.CstNodes.Rules._ALPHA._Ⰳx41ⲻ5A> _Ⰳx41ⲻ5A { get; } =
                from _Ⰳx41ⲻ5A_1 in _GeneratorV5.ManualParsers.Inners._Ⰳx41ⲻ5AParser.Instance
                select new __Generated.CstNodes.Rules._ALPHA._Ⰳx41ⲻ5A(_Ⰳx41ⲻ5A_1);

            public static Parser<__Generated.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A> _Ⰳx61ⲻ7A { get; } =
                from _Ⰳx61ⲻ7A_1 in _GeneratorV5.ManualParsers.Inners._Ⰳx61ⲻ7AParser.Instance
                select new __Generated.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(_Ⰳx61ⲻ7A_1);
        }
    }
}
