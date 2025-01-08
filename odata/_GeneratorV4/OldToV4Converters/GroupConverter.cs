namespace _GeneratorV4.OldToV4Converters
{
    using System.Linq;

    using _GeneratorV4.Abnf.CstNodes;

    public sealed class GroupConverter
    {
        private GroupConverter()
        {
        }

        public static GroupConverter Instance { get; } = new GroupConverter();

        public _group Convert(AbnfParser.CstNodes.Group group)
        {
            return new _group(
                new Inners._ʺx28ʺ(
                    x28Converter.Instance.Convert(group.OpenParenthesis)),
                group.PrefixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)),
                AlternationConverter.Instance.Convert(group.Alternation),
                group.SuffixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)),
                new Inners._ʺx29ʺ(
                    x29Converter.Instance.Convert(group.CloseParenthesis)));
        }
    }
}
