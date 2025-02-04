namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral> Instance { get; } = (_nullValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_booleanValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_guidValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_dateValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_dateTimeOffsetValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_timeOfDayValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_decimalValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_doubleValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_singleValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_sbyteValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_byteValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_int16ValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_int32ValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_int64ValueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_stringParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_durationParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_enumParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_binaryParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geographyCollectionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geographyLineStringParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geographyMultiLineStringParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geographyMultiPointParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geographyMultiPolygonParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geographyPointParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geographyPolygonParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geometryCollectionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geometryLineStringParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geometryMultiLineStringParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geometryMultiPointParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geometryMultiPolygonParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geometryPointParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral>(_geometryPolygonParser.Instance);
        
        public static class _nullValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._nullValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._nullValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._nullValue> Parse(IInput<char>? input)
                {
                    var _nullValue_1 = __GeneratedOdataV3.Parsers.Rules._nullValueParser.Instance.Parse(input);
if (!_nullValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._nullValue)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._nullValue.Instance, _nullValue_1.Remainder);
                }
            }
        }
        
        public static class _booleanValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._booleanValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._booleanValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._booleanValue> Parse(IInput<char>? input)
                {
                    var _booleanValue_1 = __GeneratedOdataV3.Parsers.Rules._booleanValueParser.Instance.Parse(input);
if (!_booleanValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._booleanValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._booleanValue(_booleanValue_1.Parsed), _booleanValue_1.Remainder);
                }
            }
        }
        
        public static class _guidValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._guidValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._guidValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._guidValue> Parse(IInput<char>? input)
                {
                    var _guidValue_1 = __GeneratedOdataV3.Parsers.Rules._guidValueParser.Instance.Parse(input);
if (!_guidValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._guidValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._guidValue(_guidValue_1.Parsed), _guidValue_1.Remainder);
                }
            }
        }
        
        public static class _dateValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._dateValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._dateValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._dateValue> Parse(IInput<char>? input)
                {
                    var _dateValue_1 = __GeneratedOdataV3.Parsers.Rules._dateValueParser.Instance.Parse(input);
if (!_dateValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._dateValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._dateValue(_dateValue_1.Parsed), _dateValue_1.Remainder);
                }
            }
        }
        
        public static class _dateTimeOffsetValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._dateTimeOffsetValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._dateTimeOffsetValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._dateTimeOffsetValue> Parse(IInput<char>? input)
                {
                    var _dateTimeOffsetValue_1 = __GeneratedOdataV3.Parsers.Rules._dateTimeOffsetValueParser.Instance.Parse(input);
if (!_dateTimeOffsetValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._dateTimeOffsetValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._dateTimeOffsetValue(_dateTimeOffsetValue_1.Parsed), _dateTimeOffsetValue_1.Remainder);
                }
            }
        }
        
        public static class _timeOfDayValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._timeOfDayValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._timeOfDayValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._timeOfDayValue> Parse(IInput<char>? input)
                {
                    var _timeOfDayValue_1 = __GeneratedOdataV3.Parsers.Rules._timeOfDayValueParser.Instance.Parse(input);
if (!_timeOfDayValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._timeOfDayValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._timeOfDayValue(_timeOfDayValue_1.Parsed), _timeOfDayValue_1.Remainder);
                }
            }
        }
        
        public static class _decimalValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._decimalValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._decimalValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._decimalValue> Parse(IInput<char>? input)
                {
                    var _decimalValue_1 = __GeneratedOdataV3.Parsers.Rules._decimalValueParser.Instance.Parse(input);
if (!_decimalValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._decimalValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._decimalValue(_decimalValue_1.Parsed), _decimalValue_1.Remainder);
                }
            }
        }
        
        public static class _doubleValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._doubleValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._doubleValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._doubleValue> Parse(IInput<char>? input)
                {
                    var _doubleValue_1 = __GeneratedOdataV3.Parsers.Rules._doubleValueParser.Instance.Parse(input);
if (!_doubleValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._doubleValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._doubleValue(_doubleValue_1.Parsed), _doubleValue_1.Remainder);
                }
            }
        }
        
        public static class _singleValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._singleValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._singleValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._singleValue> Parse(IInput<char>? input)
                {
                    var _singleValue_1 = __GeneratedOdataV3.Parsers.Rules._singleValueParser.Instance.Parse(input);
if (!_singleValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._singleValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._singleValue(_singleValue_1.Parsed), _singleValue_1.Remainder);
                }
            }
        }
        
        public static class _sbyteValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._sbyteValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._sbyteValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._sbyteValue> Parse(IInput<char>? input)
                {
                    var _sbyteValue_1 = __GeneratedOdataV3.Parsers.Rules._sbyteValueParser.Instance.Parse(input);
if (!_sbyteValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._sbyteValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._sbyteValue(_sbyteValue_1.Parsed), _sbyteValue_1.Remainder);
                }
            }
        }
        
        public static class _byteValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._byteValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._byteValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._byteValue> Parse(IInput<char>? input)
                {
                    var _byteValue_1 = __GeneratedOdataV3.Parsers.Rules._byteValueParser.Instance.Parse(input);
if (!_byteValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._byteValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._byteValue(_byteValue_1.Parsed), _byteValue_1.Remainder);
                }
            }
        }
        
        public static class _int16ValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int16Value> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int16Value>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int16Value> Parse(IInput<char>? input)
                {
                    var _int16Value_1 = __GeneratedOdataV3.Parsers.Rules._int16ValueParser.Instance.Parse(input);
if (!_int16Value_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int16Value)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int16Value(_int16Value_1.Parsed), _int16Value_1.Remainder);
                }
            }
        }
        
        public static class _int32ValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int32Value> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int32Value>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int32Value> Parse(IInput<char>? input)
                {
                    var _int32Value_1 = __GeneratedOdataV3.Parsers.Rules._int32ValueParser.Instance.Parse(input);
if (!_int32Value_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int32Value)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int32Value(_int32Value_1.Parsed), _int32Value_1.Remainder);
                }
            }
        }
        
        public static class _int64ValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int64Value> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int64Value>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int64Value> Parse(IInput<char>? input)
                {
                    var _int64Value_1 = __GeneratedOdataV3.Parsers.Rules._int64ValueParser.Instance.Parse(input);
if (!_int64Value_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int64Value)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._int64Value(_int64Value_1.Parsed), _int64Value_1.Remainder);
                }
            }
        }
        
        public static class _stringParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._string> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._string>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._string> Parse(IInput<char>? input)
                {
                    var _string_1 = __GeneratedOdataV3.Parsers.Rules._stringParser.Instance.Parse(input);
if (!_string_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._string)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._string(_string_1.Parsed), _string_1.Remainder);
                }
            }
        }
        
        public static class _durationParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._duration> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._duration>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._duration> Parse(IInput<char>? input)
                {
                    var _duration_1 = __GeneratedOdataV3.Parsers.Rules._durationParser.Instance.Parse(input);
if (!_duration_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._duration)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._duration(_duration_1.Parsed), _duration_1.Remainder);
                }
            }
        }
        
        public static class _enumParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._enum> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._enum>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._enum> Parse(IInput<char>? input)
                {
                    var _enum_1 = __GeneratedOdataV3.Parsers.Rules._enumParser.Instance.Parse(input);
if (!_enum_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._enum)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._enum(_enum_1.Parsed), _enum_1.Remainder);
                }
            }
        }
        
        public static class _binaryParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._binary> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._binary>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._binary> Parse(IInput<char>? input)
                {
                    var _binary_1 = __GeneratedOdataV3.Parsers.Rules._binaryParser.Instance.Parse(input);
if (!_binary_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._binary)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._binary(_binary_1.Parsed), _binary_1.Remainder);
                }
            }
        }
        
        public static class _geographyCollectionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyCollection> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyCollection>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyCollection> Parse(IInput<char>? input)
                {
                    var _geographyCollection_1 = __GeneratedOdataV3.Parsers.Rules._geographyCollectionParser.Instance.Parse(input);
if (!_geographyCollection_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyCollection)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyCollection(_geographyCollection_1.Parsed), _geographyCollection_1.Remainder);
                }
            }
        }
        
        public static class _geographyLineStringParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyLineString> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyLineString>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyLineString> Parse(IInput<char>? input)
                {
                    var _geographyLineString_1 = __GeneratedOdataV3.Parsers.Rules._geographyLineStringParser.Instance.Parse(input);
if (!_geographyLineString_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyLineString)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyLineString(_geographyLineString_1.Parsed), _geographyLineString_1.Remainder);
                }
            }
        }
        
        public static class _geographyMultiLineStringParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiLineString> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiLineString>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiLineString> Parse(IInput<char>? input)
                {
                    var _geographyMultiLineString_1 = __GeneratedOdataV3.Parsers.Rules._geographyMultiLineStringParser.Instance.Parse(input);
if (!_geographyMultiLineString_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiLineString)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiLineString(_geographyMultiLineString_1.Parsed), _geographyMultiLineString_1.Remainder);
                }
            }
        }
        
        public static class _geographyMultiPointParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiPoint> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiPoint>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiPoint> Parse(IInput<char>? input)
                {
                    var _geographyMultiPoint_1 = __GeneratedOdataV3.Parsers.Rules._geographyMultiPointParser.Instance.Parse(input);
if (!_geographyMultiPoint_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiPoint)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiPoint(_geographyMultiPoint_1.Parsed), _geographyMultiPoint_1.Remainder);
                }
            }
        }
        
        public static class _geographyMultiPolygonParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiPolygon> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiPolygon>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiPolygon> Parse(IInput<char>? input)
                {
                    var _geographyMultiPolygon_1 = __GeneratedOdataV3.Parsers.Rules._geographyMultiPolygonParser.Instance.Parse(input);
if (!_geographyMultiPolygon_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiPolygon)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyMultiPolygon(_geographyMultiPolygon_1.Parsed), _geographyMultiPolygon_1.Remainder);
                }
            }
        }
        
        public static class _geographyPointParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyPoint> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyPoint>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyPoint> Parse(IInput<char>? input)
                {
                    var _geographyPoint_1 = __GeneratedOdataV3.Parsers.Rules._geographyPointParser.Instance.Parse(input);
if (!_geographyPoint_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyPoint)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyPoint(_geographyPoint_1.Parsed), _geographyPoint_1.Remainder);
                }
            }
        }
        
        public static class _geographyPolygonParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyPolygon> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyPolygon>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyPolygon> Parse(IInput<char>? input)
                {
                    var _geographyPolygon_1 = __GeneratedOdataV3.Parsers.Rules._geographyPolygonParser.Instance.Parse(input);
if (!_geographyPolygon_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyPolygon)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geographyPolygon(_geographyPolygon_1.Parsed), _geographyPolygon_1.Remainder);
                }
            }
        }
        
        public static class _geometryCollectionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryCollection> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryCollection>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryCollection> Parse(IInput<char>? input)
                {
                    var _geometryCollection_1 = __GeneratedOdataV3.Parsers.Rules._geometryCollectionParser.Instance.Parse(input);
if (!_geometryCollection_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryCollection)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryCollection(_geometryCollection_1.Parsed), _geometryCollection_1.Remainder);
                }
            }
        }
        
        public static class _geometryLineStringParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryLineString> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryLineString>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryLineString> Parse(IInput<char>? input)
                {
                    var _geometryLineString_1 = __GeneratedOdataV3.Parsers.Rules._geometryLineStringParser.Instance.Parse(input);
if (!_geometryLineString_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryLineString)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryLineString(_geometryLineString_1.Parsed), _geometryLineString_1.Remainder);
                }
            }
        }
        
        public static class _geometryMultiLineStringParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiLineString> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiLineString>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiLineString> Parse(IInput<char>? input)
                {
                    var _geometryMultiLineString_1 = __GeneratedOdataV3.Parsers.Rules._geometryMultiLineStringParser.Instance.Parse(input);
if (!_geometryMultiLineString_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiLineString)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiLineString(_geometryMultiLineString_1.Parsed), _geometryMultiLineString_1.Remainder);
                }
            }
        }
        
        public static class _geometryMultiPointParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiPoint> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiPoint>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiPoint> Parse(IInput<char>? input)
                {
                    var _geometryMultiPoint_1 = __GeneratedOdataV3.Parsers.Rules._geometryMultiPointParser.Instance.Parse(input);
if (!_geometryMultiPoint_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiPoint)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiPoint(_geometryMultiPoint_1.Parsed), _geometryMultiPoint_1.Remainder);
                }
            }
        }
        
        public static class _geometryMultiPolygonParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiPolygon> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiPolygon>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiPolygon> Parse(IInput<char>? input)
                {
                    var _geometryMultiPolygon_1 = __GeneratedOdataV3.Parsers.Rules._geometryMultiPolygonParser.Instance.Parse(input);
if (!_geometryMultiPolygon_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiPolygon)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryMultiPolygon(_geometryMultiPolygon_1.Parsed), _geometryMultiPolygon_1.Remainder);
                }
            }
        }
        
        public static class _geometryPointParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryPoint> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryPoint>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryPoint> Parse(IInput<char>? input)
                {
                    var _geometryPoint_1 = __GeneratedOdataV3.Parsers.Rules._geometryPointParser.Instance.Parse(input);
if (!_geometryPoint_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryPoint)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryPoint(_geometryPoint_1.Parsed), _geometryPoint_1.Remainder);
                }
            }
        }
        
        public static class _geometryPolygonParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryPolygon> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryPolygon>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryPolygon> Parse(IInput<char>? input)
                {
                    var _geometryPolygon_1 = __GeneratedOdataV3.Parsers.Rules._geometryPolygonParser.Instance.Parse(input);
if (!_geometryPolygon_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryPolygon)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteral._geometryPolygon(_geometryPolygon_1.Parsed), _geometryPolygon_1.Remainder);
                }
            }
        }
    }
    
}
