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
        }
    }
}
