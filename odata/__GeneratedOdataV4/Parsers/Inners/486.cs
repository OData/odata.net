namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ> Instance { get; } = (_ʺx41ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ>(_ʺx51ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ>(_ʺx67ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ>(_ʺx77ʺParser.Instance);
        
        public static class _ʺx41ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx41ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx41ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx41ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx41ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx41ʺParser.Instance.Parse(input);
if (!_ʺx41ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx41ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx41ʺ.Instance, _ʺx41ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx51ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx51ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx51ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx51ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx51ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx51ʺParser.Instance.Parse(input);
if (!_ʺx51ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx51ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx51ʺ.Instance, _ʺx51ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx67ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx67ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx67ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx67ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx67ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx67ʺParser.Instance.Parse(input);
if (!_ʺx67ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx67ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx67ʺ.Instance, _ʺx67ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx77ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx77ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx77ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx77ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx77ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx77ʺParser.Instance.Parse(input);
if (!_ʺx77ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx77ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx77ʺ.Instance, _ʺx77ʺ_1.Remainder);
                }
            }
        }
    }
    
}
