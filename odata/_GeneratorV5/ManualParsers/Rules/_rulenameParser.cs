namespace _GeneratorV5.ManualParsers.Rules
{
    using Sprache;

    public static class _rulenameParser
    {
        public static Parser<__Generated.CstNodes.Rules._rulename> Instance { get; } =
            from _ALPHA_1 in _GeneratorV5.ManualParsers.Rules._ALPHAParser.Instance
            from _ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1 in _GeneratorV5.ManualParsers.Inners._ⲤALPHAⳆDIGITⳆʺx2DʺↃParser.Instance.Many()
            select new __Generated.CstNodes.Rules._rulename(_ALPHA_1, _ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1.Convert());
    }
}
