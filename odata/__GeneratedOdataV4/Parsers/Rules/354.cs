namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _preferenceParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference> Instance { get; } = (_allowEntityReferencesPreferenceParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._preference>(_callbackPreferenceParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._preference>(_continueOnErrorPreferenceParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._preference>(_includeAnnotationsPreferenceParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._preference>(_maxpagesizePreferenceParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._preference>(_respondAsyncPreferenceParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._preference>(_returnPreferenceParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._preference>(_trackChangesPreferenceParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._preference>(_waitPreferenceParser.Instance);
        
        public static class _allowEntityReferencesPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._allowEntityReferencesPreference> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._allowEntityReferencesPreference>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._preference._allowEntityReferencesPreference> Parse(IInput<char>? input)
                {
                    var _allowEntityReferencesPreference_1 = __GeneratedOdataV4.Parsers.Rules._allowEntityReferencesPreferenceParser.Instance.Parse(input);
if (!_allowEntityReferencesPreference_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._preference._allowEntityReferencesPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._preference._allowEntityReferencesPreference(_allowEntityReferencesPreference_1.Parsed), _allowEntityReferencesPreference_1.Remainder);
                }
            }
        }
        
        public static class _callbackPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._callbackPreference> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._callbackPreference>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._preference._callbackPreference> Parse(IInput<char>? input)
                {
                    var _callbackPreference_1 = __GeneratedOdataV4.Parsers.Rules._callbackPreferenceParser.Instance.Parse(input);
if (!_callbackPreference_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._preference._callbackPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._preference._callbackPreference(_callbackPreference_1.Parsed), _callbackPreference_1.Remainder);
                }
            }
        }
        
        public static class _continueOnErrorPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._continueOnErrorPreference> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._continueOnErrorPreference>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._preference._continueOnErrorPreference> Parse(IInput<char>? input)
                {
                    var _continueOnErrorPreference_1 = __GeneratedOdataV4.Parsers.Rules._continueOnErrorPreferenceParser.Instance.Parse(input);
if (!_continueOnErrorPreference_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._preference._continueOnErrorPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._preference._continueOnErrorPreference(_continueOnErrorPreference_1.Parsed), _continueOnErrorPreference_1.Remainder);
                }
            }
        }
        
        public static class _includeAnnotationsPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._includeAnnotationsPreference> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._includeAnnotationsPreference>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._preference._includeAnnotationsPreference> Parse(IInput<char>? input)
                {
                    var _includeAnnotationsPreference_1 = __GeneratedOdataV4.Parsers.Rules._includeAnnotationsPreferenceParser.Instance.Parse(input);
if (!_includeAnnotationsPreference_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._preference._includeAnnotationsPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._preference._includeAnnotationsPreference(_includeAnnotationsPreference_1.Parsed), _includeAnnotationsPreference_1.Remainder);
                }
            }
        }
        
        public static class _maxpagesizePreferenceParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._maxpagesizePreference> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._maxpagesizePreference>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._preference._maxpagesizePreference> Parse(IInput<char>? input)
                {
                    var _maxpagesizePreference_1 = __GeneratedOdataV4.Parsers.Rules._maxpagesizePreferenceParser.Instance.Parse(input);
if (!_maxpagesizePreference_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._preference._maxpagesizePreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._preference._maxpagesizePreference(_maxpagesizePreference_1.Parsed), _maxpagesizePreference_1.Remainder);
                }
            }
        }
        
        public static class _respondAsyncPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._respondAsyncPreference> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._respondAsyncPreference>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._preference._respondAsyncPreference> Parse(IInput<char>? input)
                {
                    var _respondAsyncPreference_1 = __GeneratedOdataV4.Parsers.Rules._respondAsyncPreferenceParser.Instance.Parse(input);
if (!_respondAsyncPreference_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._preference._respondAsyncPreference)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._preference._respondAsyncPreference.Instance, _respondAsyncPreference_1.Remainder);
                }
            }
        }
        
        public static class _returnPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._returnPreference> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._returnPreference>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._preference._returnPreference> Parse(IInput<char>? input)
                {
                    var _returnPreference_1 = __GeneratedOdataV4.Parsers.Rules._returnPreferenceParser.Instance.Parse(input);
if (!_returnPreference_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._preference._returnPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._preference._returnPreference(_returnPreference_1.Parsed), _returnPreference_1.Remainder);
                }
            }
        }
        
        public static class _trackChangesPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._trackChangesPreference> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._trackChangesPreference>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._preference._trackChangesPreference> Parse(IInput<char>? input)
                {
                    var _trackChangesPreference_1 = __GeneratedOdataV4.Parsers.Rules._trackChangesPreferenceParser.Instance.Parse(input);
if (!_trackChangesPreference_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._preference._trackChangesPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._preference._trackChangesPreference(_trackChangesPreference_1.Parsed), _trackChangesPreference_1.Remainder);
                }
            }
        }
        
        public static class _waitPreferenceParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._waitPreference> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._preference._waitPreference>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._preference._waitPreference> Parse(IInput<char>? input)
                {
                    var _waitPreference_1 = __GeneratedOdataV4.Parsers.Rules._waitPreferenceParser.Instance.Parse(input);
if (!_waitPreference_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._preference._waitPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._preference._waitPreference(_waitPreference_1.Parsed), _waitPreference_1.Remainder);
                }
            }
        }
    }
    
}
