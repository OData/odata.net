using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorV3.OldToNewConverters
{
    public sealed class RuleConverter
    {
        private RuleConverter()
        {
        }

        public static RuleConverter Instance { get; } = new RuleConverter();

        public GeneratorV3.Abnf._rule Convert(AbnfParser.CstNodes.Rule rule)
        {
            return new Abnf._rule(
                RuleNameConverter.Instance.Convert(rule.RuleName),
                DefinedAsConverter.Instance.Visit(rule.DefinedAs, default),
                ElementsConverter.Instance.Convert(rule.Elements),
                CnlConverter.Instance.Visit(rule.Cnl, default));
        }
    }
}
