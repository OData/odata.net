namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityOptionsParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._entityOptions> Instance { get; } = from _ⲤentityIdOption_ʺx26ʺↃ_1 in Inners._ⲤentityIdOption_ʺx26ʺↃParser.Instance.Many()
from _id_1 in __GeneratedOdataV2.Parsers.Rules._idParser.Instance
from _Ⲥʺx26ʺ_entityIdOptionↃ_1 in Inners._Ⲥʺx26ʺ_entityIdOptionↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._entityOptions(_ⲤentityIdOption_ʺx26ʺↃ_1, _id_1, _Ⲥʺx26ʺ_entityIdOptionↃ_1);
    }
    
}
