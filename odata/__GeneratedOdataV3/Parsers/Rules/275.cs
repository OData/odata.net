namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _base64b16Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._base64b16> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._base64b16>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._base64b16> Parse(IInput<char>? input)
            {
                var _base64char_1 = __GeneratedOdataV3.Parsers.Rules._base64charParser.Instance.Repeat(2, 2).Parse(input);
if (!_base64char_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._base64b16)!, input);
}

var _Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃParser.Instance.Parse(_base64char_1.Remainder);
if (!_Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._base64b16)!, input);
}

var _ʺx3Dʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3DʺParser.Instance.Optional().Parse(_Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃ_1.Remainder);
if (!_ʺx3Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._base64b16)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._base64b16(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly2<__GeneratedOdataV3.CstNodes.Rules._base64char>(_base64char_1.Parsed), _Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃ_1.Parsed,  _ʺx3Dʺ_1.Parsed.GetOrElse(null)), _ʺx3Dʺ_1.Remainder);
            }
        }
    }
    
}
