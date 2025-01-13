namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _entityOptionsParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._entityOptions> Instance { get; } = from _ⲤentityIdOption_ʺx26ʺↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤentityIdOption_ʺx26ʺↃParser.Instance.Many()
from _id_1 in __GeneratedOdata.Parsers.Rules._idParser.Instance
from _Ⲥʺx26ʺ_entityIdOptionↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx26ʺ_entityIdOptionↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._entityOptions(_ⲤentityIdOption_ʺx26ʺↃ_1, _id_1, _Ⲥʺx26ʺ_entityIdOptionↃ_1);
    }
    
}
