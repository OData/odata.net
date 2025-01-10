namespace _GeneratorV5.OldToGeneratedCstConverters
{
    using System.Linq;

    public sealed class OptionConverter
    {
        private OptionConverter()
        {
        }

        public static OptionConverter Instance { get; } = new OptionConverter();

        public __Generated.CstNodes.Rules._option Convert(AbnfParser.CstNodes.Option option)
        {
            return new __Generated.CstNodes.Rules._option(
                new __Generated.CstNodes.Inners._ʺx5Bʺ(
                    x5BConverter.Instance.Convert(option.OpenBracket)),
                option.PrefixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)),
                AlternationConverter.Instance.Convert(option.Alternation),
                option.SuffixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, default)),
                new __Generated.CstNodes.Inners._ʺx5Dʺ(
                    x5DConverter.Instance.Convert(option.CloseBracket)));
        }
    }
}
