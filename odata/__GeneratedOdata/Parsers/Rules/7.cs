namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _keyPredicateParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._keyPredicate> Instance { get; } = (_simpleKeyParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._keyPredicate>(_compoundKeyParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._keyPredicate>(_keyPathSegmentsParser.Instance);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._keyPredicate> Instance { get; } = _keyPathSegmentsParser.Instance;

        public static class _simpleKeyParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._keyPredicate._simpleKey> Instance { get; } = from _simpleKey_1 in __GeneratedOdata.Parsers.Rules._simpleKeyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._keyPredicate._simpleKey(_simpleKey_1);
        }
        
        public static class _compoundKeyParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._keyPredicate._compoundKey> Instance { get; } = from _compoundKey_1 in __GeneratedOdata.Parsers.Rules._compoundKeyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._keyPredicate._compoundKey(_compoundKey_1);
        }
        
        public static class _keyPathSegmentsParser
        {
            /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._keyPredicate._keyPathSegments> Instance { get; } = from _keyPathSegments_1 in __GeneratedOdata.Parsers.Rules._keyPathSegmentsParser.Instance
select new __GeneratedOdata.CstNodes.Rules._keyPredicate._keyPathSegments(_keyPathSegments_1);
            */
            //// PERF
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._keyPredicate._keyPathSegments> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._keyPredicate._keyPathSegments>
            {
                public IOutput<char, _keyPredicate._keyPathSegments> Parse(IInput<char>? input)
                {
                    var _keyPathSegments_1 = __GeneratedOdata.Parsers.Rules._keyPathSegmentsParser.Instance.Parse(input);
                    if (!_keyPathSegments_1.Success)
                    {
                        return Output.Create(
                            false,
                            default(_keyPredicate._keyPathSegments)!,
                            input);
                    }

                    return Output.Create(
                        true,
                        new __GeneratedOdata.CstNodes.Rules._keyPredicate._keyPathSegments(_keyPathSegments_1.Parsed),
                        _keyPathSegments_1 .Remainder);
                }
            }
        }
    }
    
}
