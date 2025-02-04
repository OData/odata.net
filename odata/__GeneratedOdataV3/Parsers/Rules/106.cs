namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _contextFragmentParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment> Instance { get; } = (_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment>(_ʺx24x72x65x66ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment>(_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment>(_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment>(_singletonEntity_꘡navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡꘡_꘡selectList꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment>(_qualifiedTypeName_꘡selectList꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment>(_entitySet_Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment>(_entitySet_keyPredicate_ʺx2Fʺ_contextPropertyPath_꘡selectList꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment>(_entitySet_꘡selectList꘡_꘡ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ꘡Parser.Instance);
        
        public static class _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺParser.Instance.Parse(input);
if (!_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺ.Instance, _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx24x72x65x66ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx24x72x65x66ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx24x72x65x66ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx24x72x65x66ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x72x65x66ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x72x65x66ʺParser.Instance.Parse(input);
if (!_ʺx24x72x65x66ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx24x72x65x66ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx24x72x65x66ʺ.Instance, _ʺx24x72x65x66ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺParser.Instance.Parse(input);
if (!_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ.Instance, _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺParser.Instance.Parse(input);
if (!_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺ.Instance, _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺ_1.Remainder);
                }
            }
        }
        
        public static class _singletonEntity_꘡navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡꘡_꘡selectList꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._singletonEntity_꘡navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡꘡_꘡selectList꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._singletonEntity_꘡navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡꘡_꘡selectList꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._singletonEntity_꘡navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡꘡_꘡selectList꘡> Parse(IInput<char>? input)
                {
                    var _singletonEntity_1 = __GeneratedOdataV3.Parsers.Rules._singletonEntityParser.Instance.Parse(input);
if (!_singletonEntity_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._singletonEntity_꘡navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡꘡_꘡selectList꘡)!, input);
}

var _navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡_1 = __GeneratedOdataV3.Parsers.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser.Instance.Optional().Parse(_singletonEntity_1.Remainder);
if (!_navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._singletonEntity_꘡navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡꘡_꘡selectList꘡)!, input);
}

var _selectList_1 = __GeneratedOdataV3.Parsers.Rules._selectListParser.Instance.Optional().Parse(_navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡_1.Remainder);
if (!_selectList_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._singletonEntity_꘡navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡꘡_꘡selectList꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._contextFragment._singletonEntity_꘡navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡꘡_꘡selectList꘡(_singletonEntity_1.Parsed, _navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡_1.Parsed.GetOrElse(null), _selectList_1.Parsed.GetOrElse(null)), _selectList_1.Remainder);
                }
            }
        }
        
        public static class _qualifiedTypeName_꘡selectList꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._qualifiedTypeName_꘡selectList꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._qualifiedTypeName_꘡selectList꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._qualifiedTypeName_꘡selectList꘡> Parse(IInput<char>? input)
                {
                    var _qualifiedTypeName_1 = __GeneratedOdataV3.Parsers.Rules._qualifiedTypeNameParser.Instance.Parse(input);
if (!_qualifiedTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._qualifiedTypeName_꘡selectList꘡)!, input);
}

var _selectList_1 = __GeneratedOdataV3.Parsers.Rules._selectListParser.Instance.Optional().Parse(_qualifiedTypeName_1.Remainder);
if (!_selectList_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._qualifiedTypeName_꘡selectList꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._contextFragment._qualifiedTypeName_꘡selectList꘡(_qualifiedTypeName_1.Parsed, _selectList_1.Parsed.GetOrElse(null)), _selectList_1.Remainder);
                }
            }
        }
        
        public static class _entitySet_Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃ> Parse(IInput<char>? input)
                {
                    var _entitySet_1 = __GeneratedOdataV3.Parsers.Rules._entitySetParser.Instance.Parse(input);
if (!_entitySet_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃ)!, input);
}

var _Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃParser.Instance.Parse(_entitySet_1.Remainder);
if (!_Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃ(_entitySet_1.Parsed, _Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃ_1.Parsed), _Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃ_1.Remainder);
                }
            }
        }
        
        public static class _entitySet_keyPredicate_ʺx2Fʺ_contextPropertyPath_꘡selectList꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_keyPredicate_ʺx2Fʺ_contextPropertyPath_꘡selectList꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_keyPredicate_ʺx2Fʺ_contextPropertyPath_꘡selectList꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_keyPredicate_ʺx2Fʺ_contextPropertyPath_꘡selectList꘡> Parse(IInput<char>? input)
                {
                    var _entitySet_1 = __GeneratedOdataV3.Parsers.Rules._entitySetParser.Instance.Parse(input);
if (!_entitySet_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_keyPredicate_ʺx2Fʺ_contextPropertyPath_꘡selectList꘡)!, input);
}

var _keyPredicate_1 = __GeneratedOdataV3.Parsers.Rules._keyPredicateParser.Instance.Parse(_entitySet_1.Remainder);
if (!_keyPredicate_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_keyPredicate_ʺx2Fʺ_contextPropertyPath_꘡selectList꘡)!, input);
}

var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(_keyPredicate_1.Remainder);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_keyPredicate_ʺx2Fʺ_contextPropertyPath_꘡selectList꘡)!, input);
}

var _contextPropertyPath_1 = __GeneratedOdataV3.Parsers.Rules._contextPropertyPathParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_contextPropertyPath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_keyPredicate_ʺx2Fʺ_contextPropertyPath_꘡selectList꘡)!, input);
}

var _selectList_1 = __GeneratedOdataV3.Parsers.Rules._selectListParser.Instance.Optional().Parse(_contextPropertyPath_1.Remainder);
if (!_selectList_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_keyPredicate_ʺx2Fʺ_contextPropertyPath_꘡selectList꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_keyPredicate_ʺx2Fʺ_contextPropertyPath_꘡selectList꘡(_entitySet_1.Parsed, _keyPredicate_1.Parsed, _ʺx2Fʺ_1.Parsed, _contextPropertyPath_1.Parsed, _selectList_1.Parsed.GetOrElse(null)), _selectList_1.Remainder);
                }
            }
        }
        
        public static class _entitySet_꘡selectList꘡_꘡ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_꘡selectList꘡_꘡ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_꘡selectList꘡_꘡ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_꘡selectList꘡_꘡ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ꘡> Parse(IInput<char>? input)
                {
                    var _entitySet_1 = __GeneratedOdataV3.Parsers.Rules._entitySetParser.Instance.Parse(input);
if (!_entitySet_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_꘡selectList꘡_꘡ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ꘡)!, input);
}

var _selectList_1 = __GeneratedOdataV3.Parsers.Rules._selectListParser.Instance.Optional().Parse(_entitySet_1.Remainder);
if (!_selectList_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_꘡selectList꘡_꘡ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ꘡)!, input);
}

var _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺParser.Instance.Optional().Parse(_selectList_1.Remainder);
if (!_ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_꘡selectList꘡_꘡ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._contextFragment._entitySet_꘡selectList꘡_꘡ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ꘡(_entitySet_1.Parsed, _selectList_1.Parsed.GetOrElse(null), _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ_1.Parsed.GetOrElse(null)), _ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ_1.Remainder);
                }
            }
        }
    }
    
}
