namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ> Instance { get; } = from _complexInUri_1 in __GeneratedOdata.Parsers.Rules._complexInUriParser.Instance
from _Ⲥvalueⲻseparator_complexInUriↃ_1 in Inners._Ⲥvalueⲻseparator_complexInUriↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ(_complexInUri_1, _Ⲥvalueⲻseparator_complexInUriↃ_1);
    }
    
}
