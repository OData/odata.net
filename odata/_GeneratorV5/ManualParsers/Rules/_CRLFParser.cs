namespace _GeneratorV5.ManualParsers.Rules
{
    using Sprache;

    public static class _CRLFParser
    {
        public static Parser<__Generated.CstNodes.Rules._CRLF> Instance { get; } =
            from _CR_1 in _GeneratorV5.ManualParsers.Rules._CRParser.Instance
            from _LF_1 in _GeneratorV5.ManualParsers.Rules._LFParser.Instance
            select new __Generated.CstNodes.Rules._CRLF(_CR_1, _LF_1);
    }
}
