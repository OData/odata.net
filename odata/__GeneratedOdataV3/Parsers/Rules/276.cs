namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _base64b8Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._base64b8> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._base64b8>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._base64b8> Parse(IInput<char>? input)
            {
                var _base64char_1 = __GeneratedOdataV3.Parsers.Rules._base64charParser.Instance.Parse(input);
if (!_base64char_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._base64b8)!, input);
}

var _Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃParser.Instance.Parse(_base64char_1.Remainder);
if (!_Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._base64b8)!, input);
}

var _ʺx3Dx3Dʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Dx3DʺParser.Instance.Optional().Parse(_Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ_1.Remainder);
if (!_ʺx3Dx3Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._base64b8)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._base64b8(_base64char_1.Parsed, _Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ_1.Parsed,  _ʺx3Dx3Dʺ_1.Parsed.GetOrElse(null)), _ʺx3Dx3Dʺ_1.Remainder);
            }
        }
    }
    
}
