namespace _GeneratorV4.OldToV4Converters
{
    using System.Linq;

    using _GeneratorV4.Abnf.CstNodes;

    public sealed class OptionConverter
    {
        private OptionConverter()
        {
        }

        public static OptionConverter Instance { get; } = new OptionConverter();

        public _option Convert(AbnfParser.CstNodes.Option option)
        {
            return new _option(
                new Inners._ʺx5Bʺ(
                    x5BConverter.Instance.Convert(option.OpenBracket)),
                option.PrefixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)),
                AlternationConverter.Instance.Convert(option.Alternation),
                option.SuffixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)),
                new Inners._ʺx5Dʺ(
                    x5DConverter.Instance.Convert(option.CloseBracket)));
        }
    }
}
