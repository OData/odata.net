namespace GeneratorV3.OldToNewConverters
{
    using System.Linq;

    using GeneratorV3.OldToNewConverters.Core;

    public sealed class OptionConverter
    {
        private OptionConverter()
        {
        }

        public static OptionConverter Instance { get; } = new OptionConverter();

        public GeneratorV3.Abnf._option Convert(AbnfParser.CstNodes.Option option)
        {
            return new Abnf._option(
                new Abnf.Inners._ʺx5Bʺ(
                    x5BConverter.Instance.Convert(option.OpenBracket)),
                option.PrefixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)),
                AlternationConverter.Instance.Convert(option.Alternation),
                option.SuffixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)),
                new Abnf.Inners._ʺx5Dʺ(
                    x5DConverter.Instance.Convert(option.CloseBracket)));
        }
    }
}
