namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexColInUriParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._complexColInUri> Instance { get; } = from _beginⲻarray_1 in __GeneratedOdataV2.Parsers.Rules._beginⲻarrayParser.Instance
from _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ_1 in __GeneratedOdataV2.Parsers.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃParser.Instance.Optional()
from _endⲻarray_1 in __GeneratedOdataV2.Parsers.Rules._endⲻarrayParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._complexColInUri(_beginⲻarray_1, _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ_1.GetOrElse(null), _endⲻarray_1);
    }
    
}
