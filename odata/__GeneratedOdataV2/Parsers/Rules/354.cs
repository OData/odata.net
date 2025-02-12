namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _preferenceParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._preference> Instance { get; } = (_allowEntityReferencesPreferenceParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._preference>(_callbackPreferenceParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._preference>(_continueOnErrorPreferenceParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._preference>(_includeAnnotationsPreferenceParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._preference>(_maxpagesizePreferenceParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._preference>(_respondAsyncPreferenceParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._preference>(_returnPreferenceParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._preference>(_trackChangesPreferenceParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._preference>(_waitPreferenceParser.Instance);
        
        public static class _allowEntityReferencesPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._preference._allowEntityReferencesPreference> Instance { get; } = from _allowEntityReferencesPreference_1 in __GeneratedOdataV2.Parsers.Rules._allowEntityReferencesPreferenceParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._preference._allowEntityReferencesPreference(_allowEntityReferencesPreference_1);
        }
        
        public static class _callbackPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._preference._callbackPreference> Instance { get; } = from _callbackPreference_1 in __GeneratedOdataV2.Parsers.Rules._callbackPreferenceParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._preference._callbackPreference(_callbackPreference_1);
        }
        
        public static class _continueOnErrorPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._preference._continueOnErrorPreference> Instance { get; } = from _continueOnErrorPreference_1 in __GeneratedOdataV2.Parsers.Rules._continueOnErrorPreferenceParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._preference._continueOnErrorPreference(_continueOnErrorPreference_1);
        }
        
        public static class _includeAnnotationsPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._preference._includeAnnotationsPreference> Instance { get; } = from _includeAnnotationsPreference_1 in __GeneratedOdataV2.Parsers.Rules._includeAnnotationsPreferenceParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._preference._includeAnnotationsPreference(_includeAnnotationsPreference_1);
        }
        
        public static class _maxpagesizePreferenceParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._preference._maxpagesizePreference> Instance { get; } = from _maxpagesizePreference_1 in __GeneratedOdataV2.Parsers.Rules._maxpagesizePreferenceParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._preference._maxpagesizePreference(_maxpagesizePreference_1);
        }
        
        public static class _respondAsyncPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._preference._respondAsyncPreference> Instance { get; } = from _respondAsyncPreference_1 in __GeneratedOdataV2.Parsers.Rules._respondAsyncPreferenceParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._preference._respondAsyncPreference.Instance;
        }
        
        public static class _returnPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._preference._returnPreference> Instance { get; } = from _returnPreference_1 in __GeneratedOdataV2.Parsers.Rules._returnPreferenceParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._preference._returnPreference(_returnPreference_1);
        }
        
        public static class _trackChangesPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._preference._trackChangesPreference> Instance { get; } = from _trackChangesPreference_1 in __GeneratedOdataV2.Parsers.Rules._trackChangesPreferenceParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._preference._trackChangesPreference(_trackChangesPreference_1);
        }
        
        public static class _waitPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._preference._waitPreference> Instance { get; } = from _waitPreference_1 in __GeneratedOdataV2.Parsers.Rules._waitPreferenceParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._preference._waitPreference(_waitPreference_1);
        }
    }
    
}
