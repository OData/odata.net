namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _userinfo_ʺx40ʺParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._userinfo_ʺx40ʺ> Instance { get; } = from _userinfo_1 in __GeneratedOdataV2.Parsers.Rules._userinfoParser.Instance
from _ʺx40ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx40ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._userinfo_ʺx40ʺ(_userinfo_1, _ʺx40ʺ_1);
    }
    
}
