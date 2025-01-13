namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _complexColInUriParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._complexColInUri> Instance { get; } = from _beginⲻarray_1 in __GeneratedOdata.Parsers.Rules._beginⲻarrayParser.Instance
from _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ_1 in __GeneratedOdata.Parsers.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃParser.Instance.Optional()
from _endⲻarray_1 in __GeneratedOdata.Parsers.Rules._endⲻarrayParser.Instance
select new __GeneratedOdata.CstNodes.Rules._complexColInUri(_beginⲻarray_1, _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ_1.GetOrElse(null), _endⲻarray_1);
    }
    
}
