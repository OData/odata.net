namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _userinfo_ʺx40ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._userinfo_ʺx40ʺ> Instance { get; } = from _userinfo_1 in __GeneratedOdata.Parsers.Rules._userinfoParser.Instance
from _ʺx40ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx40ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._userinfo_ʺx40ʺ(_userinfo_1, _ʺx40ʺ_1);
    }
    
}
