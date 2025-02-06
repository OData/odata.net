namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue> Instance { get; } = (_booleanValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_guidValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_durationValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_dateValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_dateTimeOffsetValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_timeOfDayValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_enumValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_fullCollectionLiteralParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_fullLineStringLiteralParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_fullMultiPointLiteralParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_fullMultiLineStringLiteralParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_fullMultiPolygonLiteralParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_fullPointLiteralParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_fullPolygonLiteralParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_decimalValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_doubleValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_singleValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_sbyteValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_byteValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_int16ValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_int32ValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_int64ValueParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue>(_binaryValueParser.Instance);
        
        public static class _booleanValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._booleanValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._booleanValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._booleanValue> Parse(IInput<char>? input)
                {
                    var _booleanValue_1 = __GeneratedOdataV4.Parsers.Rules._booleanValueParser.Instance.Parse(input);
if (!_booleanValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._booleanValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._booleanValue(_booleanValue_1.Parsed), _booleanValue_1.Remainder);
                }
            }
        }
        
        public static class _guidValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._guidValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._guidValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._guidValue> Parse(IInput<char>? input)
                {
                    var _guidValue_1 = __GeneratedOdataV4.Parsers.Rules._guidValueParser.Instance.Parse(input);
if (!_guidValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._guidValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._guidValue(_guidValue_1.Parsed), _guidValue_1.Remainder);
                }
            }
        }
        
        public static class _durationValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._durationValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._durationValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._durationValue> Parse(IInput<char>? input)
                {
                    var _durationValue_1 = __GeneratedOdataV4.Parsers.Rules._durationValueParser.Instance.Parse(input);
if (!_durationValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._durationValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._durationValue(_durationValue_1.Parsed), _durationValue_1.Remainder);
                }
            }
        }
        
        public static class _dateValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._dateValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._dateValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._dateValue> Parse(IInput<char>? input)
                {
                    var _dateValue_1 = __GeneratedOdataV4.Parsers.Rules._dateValueParser.Instance.Parse(input);
if (!_dateValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._dateValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._dateValue(_dateValue_1.Parsed), _dateValue_1.Remainder);
                }
            }
        }
        
        public static class _dateTimeOffsetValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._dateTimeOffsetValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._dateTimeOffsetValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._dateTimeOffsetValue> Parse(IInput<char>? input)
                {
                    var _dateTimeOffsetValue_1 = __GeneratedOdataV4.Parsers.Rules._dateTimeOffsetValueParser.Instance.Parse(input);
if (!_dateTimeOffsetValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._dateTimeOffsetValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._dateTimeOffsetValue(_dateTimeOffsetValue_1.Parsed), _dateTimeOffsetValue_1.Remainder);
                }
            }
        }
        
        public static class _timeOfDayValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._timeOfDayValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._timeOfDayValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._timeOfDayValue> Parse(IInput<char>? input)
                {
                    var _timeOfDayValue_1 = __GeneratedOdataV4.Parsers.Rules._timeOfDayValueParser.Instance.Parse(input);
if (!_timeOfDayValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._timeOfDayValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._timeOfDayValue(_timeOfDayValue_1.Parsed), _timeOfDayValue_1.Remainder);
                }
            }
        }
        
        public static class _enumValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._enumValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._enumValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._enumValue> Parse(IInput<char>? input)
                {
                    var _enumValue_1 = __GeneratedOdataV4.Parsers.Rules._enumValueParser.Instance.Parse(input);
if (!_enumValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._enumValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._enumValue(_enumValue_1.Parsed), _enumValue_1.Remainder);
                }
            }
        }
        
        public static class _fullCollectionLiteralParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullCollectionLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullCollectionLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullCollectionLiteral> Parse(IInput<char>? input)
                {
                    var _fullCollectionLiteral_1 = __GeneratedOdataV4.Parsers.Rules._fullCollectionLiteralParser.Instance.Parse(input);
if (!_fullCollectionLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullCollectionLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullCollectionLiteral(_fullCollectionLiteral_1.Parsed), _fullCollectionLiteral_1.Remainder);
                }
            }
        }
        
        public static class _fullLineStringLiteralParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullLineStringLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullLineStringLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullLineStringLiteral> Parse(IInput<char>? input)
                {
                    var _fullLineStringLiteral_1 = __GeneratedOdataV4.Parsers.Rules._fullLineStringLiteralParser.Instance.Parse(input);
if (!_fullLineStringLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullLineStringLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullLineStringLiteral(_fullLineStringLiteral_1.Parsed), _fullLineStringLiteral_1.Remainder);
                }
            }
        }
        
        public static class _fullMultiPointLiteralParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiPointLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiPointLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiPointLiteral> Parse(IInput<char>? input)
                {
                    var _fullMultiPointLiteral_1 = __GeneratedOdataV4.Parsers.Rules._fullMultiPointLiteralParser.Instance.Parse(input);
if (!_fullMultiPointLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiPointLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiPointLiteral(_fullMultiPointLiteral_1.Parsed), _fullMultiPointLiteral_1.Remainder);
                }
            }
        }
        
        public static class _fullMultiLineStringLiteralParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiLineStringLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiLineStringLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiLineStringLiteral> Parse(IInput<char>? input)
                {
                    var _fullMultiLineStringLiteral_1 = __GeneratedOdataV4.Parsers.Rules._fullMultiLineStringLiteralParser.Instance.Parse(input);
if (!_fullMultiLineStringLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiLineStringLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiLineStringLiteral(_fullMultiLineStringLiteral_1.Parsed), _fullMultiLineStringLiteral_1.Remainder);
                }
            }
        }
        
        public static class _fullMultiPolygonLiteralParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiPolygonLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiPolygonLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiPolygonLiteral> Parse(IInput<char>? input)
                {
                    var _fullMultiPolygonLiteral_1 = __GeneratedOdataV4.Parsers.Rules._fullMultiPolygonLiteralParser.Instance.Parse(input);
if (!_fullMultiPolygonLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiPolygonLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullMultiPolygonLiteral(_fullMultiPolygonLiteral_1.Parsed), _fullMultiPolygonLiteral_1.Remainder);
                }
            }
        }
        
        public static class _fullPointLiteralParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullPointLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullPointLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullPointLiteral> Parse(IInput<char>? input)
                {
                    var _fullPointLiteral_1 = __GeneratedOdataV4.Parsers.Rules._fullPointLiteralParser.Instance.Parse(input);
if (!_fullPointLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullPointLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullPointLiteral(_fullPointLiteral_1.Parsed), _fullPointLiteral_1.Remainder);
                }
            }
        }
        
        public static class _fullPolygonLiteralParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullPolygonLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullPolygonLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullPolygonLiteral> Parse(IInput<char>? input)
                {
                    var _fullPolygonLiteral_1 = __GeneratedOdataV4.Parsers.Rules._fullPolygonLiteralParser.Instance.Parse(input);
if (!_fullPolygonLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullPolygonLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._fullPolygonLiteral(_fullPolygonLiteral_1.Parsed), _fullPolygonLiteral_1.Remainder);
                }
            }
        }
        
        public static class _decimalValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._decimalValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._decimalValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._decimalValue> Parse(IInput<char>? input)
                {
                    var _decimalValue_1 = __GeneratedOdataV4.Parsers.Rules._decimalValueParser.Instance.Parse(input);
if (!_decimalValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._decimalValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._decimalValue(_decimalValue_1.Parsed), _decimalValue_1.Remainder);
                }
            }
        }
        
        public static class _doubleValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._doubleValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._doubleValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._doubleValue> Parse(IInput<char>? input)
                {
                    var _doubleValue_1 = __GeneratedOdataV4.Parsers.Rules._doubleValueParser.Instance.Parse(input);
if (!_doubleValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._doubleValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._doubleValue(_doubleValue_1.Parsed), _doubleValue_1.Remainder);
                }
            }
        }
        
        public static class _singleValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._singleValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._singleValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._singleValue> Parse(IInput<char>? input)
                {
                    var _singleValue_1 = __GeneratedOdataV4.Parsers.Rules._singleValueParser.Instance.Parse(input);
if (!_singleValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._singleValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._singleValue(_singleValue_1.Parsed), _singleValue_1.Remainder);
                }
            }
        }
        
        public static class _sbyteValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._sbyteValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._sbyteValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._sbyteValue> Parse(IInput<char>? input)
                {
                    var _sbyteValue_1 = __GeneratedOdataV4.Parsers.Rules._sbyteValueParser.Instance.Parse(input);
if (!_sbyteValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._sbyteValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._sbyteValue(_sbyteValue_1.Parsed), _sbyteValue_1.Remainder);
                }
            }
        }
        
        public static class _byteValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._byteValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._byteValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._byteValue> Parse(IInput<char>? input)
                {
                    var _byteValue_1 = __GeneratedOdataV4.Parsers.Rules._byteValueParser.Instance.Parse(input);
if (!_byteValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._byteValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._byteValue(_byteValue_1.Parsed), _byteValue_1.Remainder);
                }
            }
        }
        
        public static class _int16ValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._int16Value> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._int16Value>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._int16Value> Parse(IInput<char>? input)
                {
                    var _int16Value_1 = __GeneratedOdataV4.Parsers.Rules._int16ValueParser.Instance.Parse(input);
if (!_int16Value_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._int16Value)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._int16Value(_int16Value_1.Parsed), _int16Value_1.Remainder);
                }
            }
        }
        
        public static class _int32ValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._int32Value> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._int32Value>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._int32Value> Parse(IInput<char>? input)
                {
                    var _int32Value_1 = __GeneratedOdataV4.Parsers.Rules._int32ValueParser.Instance.Parse(input);
if (!_int32Value_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._int32Value)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._int32Value(_int32Value_1.Parsed), _int32Value_1.Remainder);
                }
            }
        }
        
        public static class _int64ValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._int64Value> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._int64Value>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._int64Value> Parse(IInput<char>? input)
                {
                    var _int64Value_1 = __GeneratedOdataV4.Parsers.Rules._int64ValueParser.Instance.Parse(input);
if (!_int64Value_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._int64Value)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._int64Value(_int64Value_1.Parsed), _int64Value_1.Remainder);
                }
            }
        }
        
        public static class _binaryValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._binaryValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._binaryValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveValue._binaryValue> Parse(IInput<char>? input)
                {
                    var _binaryValue_1 = __GeneratedOdataV4.Parsers.Rules._binaryValueParser.Instance.Parse(input);
if (!_binaryValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveValue._binaryValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveValue._binaryValue(_binaryValue_1.Parsed), _binaryValue_1.Remainder);
                }
            }
        }
    }
    
}
