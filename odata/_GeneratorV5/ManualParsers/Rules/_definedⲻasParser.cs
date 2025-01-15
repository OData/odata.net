namespace _GeneratorV5.ManualParsers.Rules
{
    using __Generated.CstNodes.Inners;
    using Sprache;

    public static class _definedⲻasParser
    {
        public static Parser<__Generated.CstNodes.Rules._definedⲻas> Instance { get; } =
            from _cⲻwsp_1 in _GeneratorV5.ManualParsers.Rules._cⲻwspParser.Instance.Many()
            from _Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1 in _GeneratorV5.ManualParsers.Inners._Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1Parser.Instance
            from _cⲻwsp_2 in _GeneratorV5.ManualParsers.Rules._cⲻwspParser.Instance.Many()
            select new __Generated.CstNodes.Rules._definedⲻas(_cⲻwsp_1.Convert(), _Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1, _cⲻwsp_2.Convert());

        public static HelperRangedAtMost0<T> Convert<T>(this System.Collections.Generic.IEnumerable<T> source)
        {
            return new HelperRangedAtMost0<T>(source);
        }

        public static HelperRangedAtLeast1<T> Convert2<T>(this System.Collections.Generic.IEnumerable<T> source)
        {
            return new HelperRangedAtLeast1<T>(source);
        }
    }
}
