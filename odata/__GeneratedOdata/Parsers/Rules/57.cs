namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _entityCastOptionsParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._entityCastOptions> Instance { get; } = from _ⲤentityCastOption_ʺx26ʺↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤentityCastOption_ʺx26ʺↃParser.Instance.Many()
from _id_1 in __GeneratedOdata.Parsers.Rules._idParser.Instance
from _Ⲥʺx26ʺ_entityCastOptionↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx26ʺ_entityCastOptionↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._entityCastOptions(_ⲤentityCastOption_ʺx26ʺↃ_1, _id_1, _Ⲥʺx26ʺ_entityCastOptionↃ_1);
    }
    
}