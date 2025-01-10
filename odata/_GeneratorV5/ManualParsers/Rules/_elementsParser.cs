namespace _GeneratorV5.ManualParsers.Rules
{
    using Sprache;

    public static class _elementsParser
    {
        public static Parser<__Generated.CstNodes.Rules._elements> Instance { get; } =
            from _alternation_1 in _GeneratorV5.ManualParsers.Rules._alternationParser.Instance
            from _cⲻwsp_1 in _GeneratorV5.ManualParsers.Rules._cⲻwspParser.Instance.Many()
            select new __Generated.CstNodes.Rules._elements(_alternation_1, _cⲻwsp_1);
    }
}
