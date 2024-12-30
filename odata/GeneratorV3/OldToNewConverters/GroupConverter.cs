namespace GeneratorV3.OldToNewConverters
{
    using System.Linq;

    using GeneratorV3.OldToNewConverters.Core;

    public sealed class GroupConverter
    {
        private GroupConverter()
        {
        }

        public static GroupConverter Instance { get; } = new GroupConverter();

        public GeneratorV3.Abnf._group Convert(AbnfParser.CstNodes.Group group)
        {
            return new Abnf._group(
                new Abnf.Inners._doublequotex28doublequote(
                    x28Converter.Instance.Convert(group.OpenParenthesis)),
                group.PrefixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)),
                AlternationConverter.Instance.Convert(group.Alternation),
                group.SuffixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)),
                new Abnf.Inners._doublequotex29doublequote(
                    x29Converter.Instance.Convert(group.CloseParenthesis)));
        }
    }
}
