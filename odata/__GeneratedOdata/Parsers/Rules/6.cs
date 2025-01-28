namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _collectionNavPathParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath> Instance { get; } = (_keyPredicate_꘡singleNavigation꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath>(_filterInPath_꘡collectionNavigation꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath>(_each_꘡boundOperation꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath>(_boundOperationParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath>(_countParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath>(_refParser.Instance);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath> Instance { get; } = _keyPredicate_꘡singleNavigation꘡Parser.Instance;

        public static class _keyPredicate_꘡singleNavigation꘡Parser
        {
            /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡> Instance { get; } = from _keyPredicate_1 in __GeneratedOdata.Parsers.Rules._keyPredicateParser.Instance
                                                                                                                                                 from _singleNavigation_1 in __GeneratedOdata.Parsers.Rules._singleNavigationParser.Instance.Optional()
                                                                                                                                                 select new __GeneratedOdata.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡(_keyPredicate_1, _singleNavigation_1.GetOrElse(null));
            */
            //// PERF
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡>
            {
                public IOutput<char, _collectionNavPath._keyPredicate_꘡singleNavigation꘡> Parse(IInput<char>? input)
                {
                    var _keyPredicate_1 = __GeneratedOdata.Parsers.Rules._keyPredicateParser.Instance.Parse(input);
                    var _singleNavigation_1 = __GeneratedOdata.Parsers.Rules._singleNavigationParser.Instance.Optional().Parse(_keyPredicate_1.Remainder);
                    return Output.Create(
                        true, 
                        new __GeneratedOdata.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡(_keyPredicate_1.Parsed, _singleNavigation_1.Parsed.GetOrElse(null)),
                        _singleNavigation_1.Remainder);
                }
            }
        }
        
        public static class _filterInPath_꘡collectionNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath._filterInPath_꘡collectionNavigation꘡> Instance { get; } = from _filterInPath_1 in __GeneratedOdata.Parsers.Rules._filterInPathParser.Instance
from _collectionNavigation_1 in __GeneratedOdata.Parsers.Rules._collectionNavigationParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._collectionNavPath._filterInPath_꘡collectionNavigation꘡(_filterInPath_1, _collectionNavigation_1.GetOrElse(null));
        }
        
        public static class _each_꘡boundOperation꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath._each_꘡boundOperation꘡> Instance { get; } = from _each_1 in __GeneratedOdata.Parsers.Rules._eachParser.Instance
from _boundOperation_1 in __GeneratedOdata.Parsers.Rules._boundOperationParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._collectionNavPath._each_꘡boundOperation꘡(_each_1, _boundOperation_1.GetOrElse(null));
        }
        
        public static class _boundOperationParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath._boundOperation> Instance { get; } = from _boundOperation_1 in __GeneratedOdata.Parsers.Rules._boundOperationParser.Instance
select new __GeneratedOdata.CstNodes.Rules._collectionNavPath._boundOperation(_boundOperation_1);
        }
        
        public static class _countParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath._count> Instance { get; } = from _count_1 in __GeneratedOdata.Parsers.Rules._countParser.Instance
select new __GeneratedOdata.CstNodes.Rules._collectionNavPath._count(_count_1);
        }
        
        public static class _refParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavPath._ref> Instance { get; } = from _ref_1 in __GeneratedOdata.Parsers.Rules._refParser.Instance
select new __GeneratedOdata.CstNodes.Rules._collectionNavPath._ref(_ref_1);
        }
    }
    
}
