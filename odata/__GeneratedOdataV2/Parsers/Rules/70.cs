namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _levelsParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._levels> Instance { get; } = from _Ⲥʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺↃParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._levels(_Ⲥʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺↃ_1, _EQ_1, _ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃ_1);
    }
    
}
