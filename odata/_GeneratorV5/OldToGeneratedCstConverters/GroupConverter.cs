namespace _GeneratorV5.OldToGeneratedCstConverters
{
    using _GeneratorV5.ManualParsers.Rules;
    using System.Linq;

    public sealed class GroupConverter
    {
        private GroupConverter()
        {
        }

        public static GroupConverter Instance { get; } = new GroupConverter();

        public __Generated.CstNodes.Rules._group Convert(AbnfParser.CstNodes.Group group)
        {
            return new __Generated.CstNodes.Rules._group(
                new __Generated.CstNodes.Inners._ʺx28ʺ(
                    x28Converter.Instance.Convert(group.OpenParenthesis)),
                group.PrefixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)).Convert(),
                AlternationConverter.Instance.Convert(group.Alternation),
                group.SuffixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)).Convert(),
                new __Generated.CstNodes.Inners._ʺx29ʺ(
                    x29Converter.Instance.Convert(group.CloseParenthesis)));
        }
    }
}
