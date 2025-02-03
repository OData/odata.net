namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _base64b8Parser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._base64b8> Instance { get; } = from _base64char_1 in __GeneratedOdataV2.Parsers.Rules._base64charParser.Instance
from _Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃParser.Instance
from _ʺx3Dx3Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3Dx3DʺParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._base64b8(_base64char_1, _Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ_1, _ʺx3Dx3Dʺ_1.GetOrElse(null));
    }
    
}
