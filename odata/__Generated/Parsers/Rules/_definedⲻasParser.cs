namespace __Generated.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _definedⲻasParser
    {
        public static IParser<char, __Generated.CstNodes.Rules._definedⲻas> Instance { get; } = from _cⲻwsp_1 in __Generated.Parsers.Rules._cⲻwspParser.Instance.Many()
from _Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1 in __Generated.Parsers.Inners._Ⲥʺx3DʺⳆʺx3Dx2FʺↃParser.Instance
from _cⲻwsp_2 in __Generated.Parsers.Rules._cⲻwspParser.Instance.Many()
select new __Generated.CstNodes.Rules._definedⲻas(_cⲻwsp_1, _Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1, _cⲻwsp_2);
    }
    
}
