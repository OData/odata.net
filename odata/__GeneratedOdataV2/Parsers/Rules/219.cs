namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _numberInJSONParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._numberInJSON> Instance { get; } = from _ʺx2Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2DʺParser.Instance.Optional()
from _int_1 in __GeneratedOdataV2.Parsers.Rules._intParser.Instance
from _frac_1 in __GeneratedOdataV2.Parsers.Rules._fracParser.Instance.Optional()
from _exp_1 in __GeneratedOdataV2.Parsers.Rules._expParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._numberInJSON(_ʺx2Dʺ_1.GetOrElse(null), _int_1, _frac_1.GetOrElse(null), _exp_1.GetOrElse(null));
    }
    
}
