namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _base64b8Parser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._base64b8> Instance { get; } = from _base64char_1 in __GeneratedOdata.Parsers.Rules._base64charParser.Instance
from _Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃParser.Instance
from _ʺx3Dx3Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3Dx3DʺParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._base64b8(_base64char_1, _Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ_1, _ʺx3Dx3Dʺ_1.GetOrElse(null));
    }
    
}
