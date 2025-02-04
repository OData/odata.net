namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _methodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr> Instance { get; } = (_indexOfMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_toLowerMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_toUpperMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_trimMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_substringMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_concatMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_lengthMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_yearMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_monthMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_dayMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_hourMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_minuteMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_secondMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_fractionalsecondsMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_totalsecondsMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_dateMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_timeMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_roundMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_floorMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_ceilingMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_distanceMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_geoLengthMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_totalOffsetMinutesMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_minDateTimeMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_maxDateTimeMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_nowMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_boolMethodCallExprParser.Instance);
        
        public static class _indexOfMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._indexOfMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._indexOfMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._indexOfMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _indexOfMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._indexOfMethodCallExprParser.Instance.Parse(input);
if (!_indexOfMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._indexOfMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._indexOfMethodCallExpr(_indexOfMethodCallExpr_1.Parsed), _indexOfMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _toLowerMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toLowerMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toLowerMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toLowerMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _toLowerMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._toLowerMethodCallExprParser.Instance.Parse(input);
if (!_toLowerMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toLowerMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toLowerMethodCallExpr(_toLowerMethodCallExpr_1.Parsed), _toLowerMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _toUpperMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toUpperMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toUpperMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toUpperMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _toUpperMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._toUpperMethodCallExprParser.Instance.Parse(input);
if (!_toUpperMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toUpperMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toUpperMethodCallExpr(_toUpperMethodCallExpr_1.Parsed), _toUpperMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _trimMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._trimMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._trimMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._trimMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _trimMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._trimMethodCallExprParser.Instance.Parse(input);
if (!_trimMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._trimMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._trimMethodCallExpr(_trimMethodCallExpr_1.Parsed), _trimMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _substringMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._substringMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._substringMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._substringMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _substringMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._substringMethodCallExprParser.Instance.Parse(input);
if (!_substringMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._substringMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._substringMethodCallExpr(_substringMethodCallExpr_1.Parsed), _substringMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _concatMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._concatMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._concatMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._concatMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _concatMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._concatMethodCallExprParser.Instance.Parse(input);
if (!_concatMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._concatMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._concatMethodCallExpr(_concatMethodCallExpr_1.Parsed), _concatMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _lengthMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._lengthMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._lengthMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._lengthMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _lengthMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._lengthMethodCallExprParser.Instance.Parse(input);
if (!_lengthMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._lengthMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._lengthMethodCallExpr(_lengthMethodCallExpr_1.Parsed), _lengthMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _yearMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._yearMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._yearMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._yearMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _yearMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._yearMethodCallExprParser.Instance.Parse(input);
if (!_yearMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._yearMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._yearMethodCallExpr(_yearMethodCallExpr_1.Parsed), _yearMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _monthMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._monthMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._monthMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._monthMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _monthMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._monthMethodCallExprParser.Instance.Parse(input);
if (!_monthMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._monthMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._monthMethodCallExpr(_monthMethodCallExpr_1.Parsed), _monthMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _dayMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dayMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dayMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dayMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _dayMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._dayMethodCallExprParser.Instance.Parse(input);
if (!_dayMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dayMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dayMethodCallExpr(_dayMethodCallExpr_1.Parsed), _dayMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _hourMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._hourMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._hourMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._hourMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _hourMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._hourMethodCallExprParser.Instance.Parse(input);
if (!_hourMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._hourMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._hourMethodCallExpr(_hourMethodCallExpr_1.Parsed), _hourMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _minuteMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minuteMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minuteMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minuteMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _minuteMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._minuteMethodCallExprParser.Instance.Parse(input);
if (!_minuteMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minuteMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minuteMethodCallExpr(_minuteMethodCallExpr_1.Parsed), _minuteMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _secondMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._secondMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._secondMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._secondMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _secondMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._secondMethodCallExprParser.Instance.Parse(input);
if (!_secondMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._secondMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._secondMethodCallExpr(_secondMethodCallExpr_1.Parsed), _secondMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _fractionalsecondsMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._fractionalsecondsMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._fractionalsecondsMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._fractionalsecondsMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _fractionalsecondsMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._fractionalsecondsMethodCallExprParser.Instance.Parse(input);
if (!_fractionalsecondsMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._fractionalsecondsMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._fractionalsecondsMethodCallExpr(_fractionalsecondsMethodCallExpr_1.Parsed), _fractionalsecondsMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _totalsecondsMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalsecondsMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalsecondsMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalsecondsMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _totalsecondsMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._totalsecondsMethodCallExprParser.Instance.Parse(input);
if (!_totalsecondsMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalsecondsMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalsecondsMethodCallExpr(_totalsecondsMethodCallExpr_1.Parsed), _totalsecondsMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _dateMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dateMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dateMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dateMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _dateMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._dateMethodCallExprParser.Instance.Parse(input);
if (!_dateMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dateMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dateMethodCallExpr(_dateMethodCallExpr_1.Parsed), _dateMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _timeMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._timeMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._timeMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._timeMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _timeMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._timeMethodCallExprParser.Instance.Parse(input);
if (!_timeMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._timeMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._timeMethodCallExpr(_timeMethodCallExpr_1.Parsed), _timeMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _roundMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._roundMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._roundMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._roundMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _roundMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._roundMethodCallExprParser.Instance.Parse(input);
if (!_roundMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._roundMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._roundMethodCallExpr(_roundMethodCallExpr_1.Parsed), _roundMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _floorMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._floorMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._floorMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._floorMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _floorMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._floorMethodCallExprParser.Instance.Parse(input);
if (!_floorMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._floorMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._floorMethodCallExpr(_floorMethodCallExpr_1.Parsed), _floorMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _ceilingMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._ceilingMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._ceilingMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._ceilingMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _ceilingMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._ceilingMethodCallExprParser.Instance.Parse(input);
if (!_ceilingMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._ceilingMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._ceilingMethodCallExpr(_ceilingMethodCallExpr_1.Parsed), _ceilingMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _distanceMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._distanceMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._distanceMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._distanceMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _distanceMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._distanceMethodCallExprParser.Instance.Parse(input);
if (!_distanceMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._distanceMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._distanceMethodCallExpr(_distanceMethodCallExpr_1.Parsed), _distanceMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _geoLengthMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._geoLengthMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._geoLengthMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._geoLengthMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _geoLengthMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._geoLengthMethodCallExprParser.Instance.Parse(input);
if (!_geoLengthMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._geoLengthMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._geoLengthMethodCallExpr(_geoLengthMethodCallExpr_1.Parsed), _geoLengthMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _totalOffsetMinutesMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalOffsetMinutesMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalOffsetMinutesMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalOffsetMinutesMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _totalOffsetMinutesMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._totalOffsetMinutesMethodCallExprParser.Instance.Parse(input);
if (!_totalOffsetMinutesMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalOffsetMinutesMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalOffsetMinutesMethodCallExpr(_totalOffsetMinutesMethodCallExpr_1.Parsed), _totalOffsetMinutesMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _minDateTimeMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minDateTimeMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minDateTimeMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minDateTimeMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _minDateTimeMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._minDateTimeMethodCallExprParser.Instance.Parse(input);
if (!_minDateTimeMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minDateTimeMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minDateTimeMethodCallExpr(_minDateTimeMethodCallExpr_1.Parsed), _minDateTimeMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _maxDateTimeMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._maxDateTimeMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._maxDateTimeMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._maxDateTimeMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _maxDateTimeMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._maxDateTimeMethodCallExprParser.Instance.Parse(input);
if (!_maxDateTimeMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._maxDateTimeMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._maxDateTimeMethodCallExpr(_maxDateTimeMethodCallExpr_1.Parsed), _maxDateTimeMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _nowMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._nowMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._nowMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._nowMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _nowMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._nowMethodCallExprParser.Instance.Parse(input);
if (!_nowMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._nowMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._nowMethodCallExpr(_nowMethodCallExpr_1.Parsed), _nowMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _boolMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._boolMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._boolMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._boolMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _boolMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._boolMethodCallExprParser.Instance.Parse(input);
if (!_boolMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._methodCallExpr._boolMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._boolMethodCallExpr(_boolMethodCallExpr_1.Parsed), _boolMethodCallExpr_1.Remainder);
                }
            }
        }
    }
    
}
