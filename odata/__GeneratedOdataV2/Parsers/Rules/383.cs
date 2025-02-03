namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _URIParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._URI> Instance { get; } = from _scheme_1 in __GeneratedOdataV2.Parsers.Rules._schemeParser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3AʺParser.Instance
from _hierⲻpart_1 in __GeneratedOdataV2.Parsers.Rules._hierⲻpartParser.Instance
from _ʺx3Fʺ_query_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3Fʺ_queryParser.Instance.Optional()
from _ʺx23ʺ_fragment_1 in __GeneratedOdataV2.Parsers.Inners._ʺx23ʺ_fragmentParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._URI(_scheme_1, _ʺx3Aʺ_1, _hierⲻpart_1, _ʺx3Fʺ_query_1.GetOrElse(null), _ʺx23ʺ_fragment_1.GetOrElse(null));
    }
    
}
