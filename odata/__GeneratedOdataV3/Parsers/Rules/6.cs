namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionNavPathParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath> Instance { get; } = (_keyPredicate_꘡singleNavigation꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath>(_filterInPath_꘡collectionNavigation꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath>(_each_꘡boundOperation꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath>(_boundOperationParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath>(_countParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath>(_refParser.Instance);
        
        public static class _keyPredicate_꘡singleNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡> Parse(IInput<char>? input)
                {
                    var _keyPredicate_1 = __GeneratedOdataV3.Parsers.Rules._keyPredicateParser.Instance.Parse(input);
if (!_keyPredicate_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡)!, input);
}

var _singleNavigation_1 = __GeneratedOdataV3.Parsers.Rules._singleNavigationParser.Instance.Optional().Parse(_keyPredicate_1.Remainder);
if (!_singleNavigation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡(_keyPredicate_1.Parsed, _singleNavigation_1.Parsed.GetOrElse(null)), _singleNavigation_1.Remainder);
                }
            }
        }
        
        public static class _filterInPath_꘡collectionNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._filterInPath_꘡collectionNavigation꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._filterInPath_꘡collectionNavigation꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._filterInPath_꘡collectionNavigation꘡> Parse(IInput<char>? input)
                {
                    var _filterInPath_1 = __GeneratedOdataV3.Parsers.Rules._filterInPathParser.Instance.Parse(input);
if (!_filterInPath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPath._filterInPath_꘡collectionNavigation꘡)!, input);
}

var _collectionNavigation_1 = __GeneratedOdataV3.Parsers.Rules._collectionNavigationParser.Instance.Optional().Parse(_filterInPath_1.Remainder);
if (!_collectionNavigation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPath._filterInPath_꘡collectionNavigation꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._filterInPath_꘡collectionNavigation꘡(_filterInPath_1.Parsed, _collectionNavigation_1.Parsed.GetOrElse(null)), _collectionNavigation_1.Remainder);
                }
            }
        }
        
        public static class _each_꘡boundOperation꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._each_꘡boundOperation꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._each_꘡boundOperation꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._each_꘡boundOperation꘡> Parse(IInput<char>? input)
                {
                    var _each_1 = __GeneratedOdataV3.Parsers.Rules._eachParser.Instance.Parse(input);
if (!_each_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPath._each_꘡boundOperation꘡)!, input);
}

var _boundOperation_1 = __GeneratedOdataV3.Parsers.Rules._boundOperationParser.Instance.Optional().Parse(_each_1.Remainder);
if (!_boundOperation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPath._each_꘡boundOperation꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._each_꘡boundOperation꘡(_each_1.Parsed, _boundOperation_1.Parsed.GetOrElse(null)), _boundOperation_1.Remainder);
                }
            }
        }
        
        public static class _boundOperationParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._boundOperation> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._boundOperation>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._boundOperation> Parse(IInput<char>? input)
                {
                    var _boundOperation_1 = __GeneratedOdataV3.Parsers.Rules._boundOperationParser.Instance.Parse(input);
if (!_boundOperation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPath._boundOperation)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._boundOperation(_boundOperation_1.Parsed), _boundOperation_1.Remainder);
                }
            }
        }
        
        public static class _countParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._count> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._count>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._count> Parse(IInput<char>? input)
                {
                    var _count_1 = __GeneratedOdataV3.Parsers.Rules._countParser.Instance.Parse(input);
if (!_count_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPath._count)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._count(_count_1.Parsed), _count_1.Remainder);
                }
            }
        }
        
        public static class _refParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._ref> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._ref>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._ref> Parse(IInput<char>? input)
                {
                    var _ref_1 = __GeneratedOdataV3.Parsers.Rules._refParser.Instance.Parse(input);
if (!_ref_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPath._ref)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._collectionNavPath._ref(_ref_1.Parsed), _ref_1.Remainder);
                }
            }
        }
    }
    
}
