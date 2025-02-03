namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _base64b16Parser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._base64b16> Instance { get; } = from _base64char_1 in __GeneratedOdataV2.Parsers.Rules._base64charParser.Instance.Repeat(2, 2)
from _Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃParser.Instance
from _ʺx3Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3DʺParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._base64b16(new __GeneratedOdataV2.CstNodes.Inners.HelperRangedExactly2<__GeneratedOdataV2.CstNodes.Rules._base64char>(_base64char_1), _Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃ_1, _ʺx3Dʺ_1.GetOrElse(null));
    }
    
}
