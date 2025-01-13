namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _base64b16Parser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._base64b16> Instance { get; } = from _base64char_1 in __GeneratedOdata.Parsers.Rules._base64charParser.Instance.Many()
from _Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃParser.Instance
from _ʺx3Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3DʺParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._base64b16(_base64char_1, _Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃ_1, _ʺx3Dʺ_1.GetOrElse(null));
    }
    
}
