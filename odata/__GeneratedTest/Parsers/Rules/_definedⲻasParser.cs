namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _definedⲻasParser
    {
        public static Parser<__GeneratedTest.CstNodes.Rules._definedⲻas> Instance { get; } = from _cⲻwsp_1 in __GeneratedTest.Parsers.Rules._cⲻwspParser.Instance.Many()
from _Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1 in __GeneratedTest.Parsers.Inners._Ⲥʺx3DʺⳆʺx3Dx2FʺↃParser.Instance
from _cⲻwsp_2 in __GeneratedTest.Parsers.Rules._cⲻwspParser.Instance.Many()
select new __GeneratedTest.CstNodes.Rules._definedⲻas(_cⲻwsp_1, _Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1, _cⲻwsp_2);
    }
    
}
