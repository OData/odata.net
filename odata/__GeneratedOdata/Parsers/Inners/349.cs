namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ> Instance { get; } = from _complexInUri_1 in __GeneratedOdata.Parsers.Rules._complexInUriParser.Instance
from _Ⲥvalueⲻseparator_complexInUriↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥvalueⲻseparator_complexInUriↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ(_complexInUri_1, _Ⲥvalueⲻseparator_complexInUriↃ_1);
    }
    
}
