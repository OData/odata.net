namespace _GeneratorV5.ManualParsers.Rules
{
    using Sprache;

    public static class _definedⲻasParser
    {
        public static Parser<__Generated.CstNodes.Rules._definedⲻas> Instance { get; } =
            from _cⲻwsp_1 in _GeneratorV5.ManualParsers.Rules._cⲻwspParser.Instance.Many()
            from _Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1 in _GeneratorV5.ManualParsers.Inners._Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1Parser.Instance
            from _cⲻwsp_2 in _GeneratorV5.ManualParsers.Rules._cⲻwspParser.Instance.Many()
            select new __Generated.CstNodes.Rules._definedⲻas(_cⲻwsp_1, _Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1, _cⲻwsp_2);
    }
}
