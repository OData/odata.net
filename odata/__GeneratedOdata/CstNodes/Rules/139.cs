namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _methodCallExpr
    {
        private _methodCallExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_methodCallExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_methodCallExpr._indexOfMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._toLowerMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._toUpperMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._trimMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._substringMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._concatMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._lengthMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._yearMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._monthMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._dayMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._hourMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._minuteMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._secondMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._fractionalsecondsMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._totalsecondsMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._dateMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._timeMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._roundMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._floorMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._ceilingMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._distanceMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._geoLengthMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._totalOffsetMinutesMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._minDateTimeMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._maxDateTimeMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._nowMethodCallExpr node, TContext context);
            protected internal abstract TResult Accept(_methodCallExpr._boolMethodCallExpr node, TContext context);
        }
        
        public sealed class _indexOfMethodCallExpr : _methodCallExpr
        {
            public _indexOfMethodCallExpr(__GeneratedOdata.CstNodes.Rules._indexOfMethodCallExpr _indexOfMethodCallExpr_1)
            {
                this._indexOfMethodCallExpr_1 = _indexOfMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._indexOfMethodCallExpr _indexOfMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _toLowerMethodCallExpr : _methodCallExpr
        {
            public _toLowerMethodCallExpr(__GeneratedOdata.CstNodes.Rules._toLowerMethodCallExpr _toLowerMethodCallExpr_1)
            {
                this._toLowerMethodCallExpr_1 = _toLowerMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._toLowerMethodCallExpr _toLowerMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _toUpperMethodCallExpr : _methodCallExpr
        {
            public _toUpperMethodCallExpr(__GeneratedOdata.CstNodes.Rules._toUpperMethodCallExpr _toUpperMethodCallExpr_1)
            {
                this._toUpperMethodCallExpr_1 = _toUpperMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._toUpperMethodCallExpr _toUpperMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _trimMethodCallExpr : _methodCallExpr
        {
            public _trimMethodCallExpr(__GeneratedOdata.CstNodes.Rules._trimMethodCallExpr _trimMethodCallExpr_1)
            {
                this._trimMethodCallExpr_1 = _trimMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._trimMethodCallExpr _trimMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _substringMethodCallExpr : _methodCallExpr
        {
            public _substringMethodCallExpr(__GeneratedOdata.CstNodes.Rules._substringMethodCallExpr _substringMethodCallExpr_1)
            {
                this._substringMethodCallExpr_1 = _substringMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._substringMethodCallExpr _substringMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _concatMethodCallExpr : _methodCallExpr
        {
            public _concatMethodCallExpr(__GeneratedOdata.CstNodes.Rules._concatMethodCallExpr _concatMethodCallExpr_1)
            {
                this._concatMethodCallExpr_1 = _concatMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._concatMethodCallExpr _concatMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _lengthMethodCallExpr : _methodCallExpr
        {
            public _lengthMethodCallExpr(__GeneratedOdata.CstNodes.Rules._lengthMethodCallExpr _lengthMethodCallExpr_1)
            {
                this._lengthMethodCallExpr_1 = _lengthMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._lengthMethodCallExpr _lengthMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _yearMethodCallExpr : _methodCallExpr
        {
            public _yearMethodCallExpr(__GeneratedOdata.CstNodes.Rules._yearMethodCallExpr _yearMethodCallExpr_1)
            {
                this._yearMethodCallExpr_1 = _yearMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._yearMethodCallExpr _yearMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _monthMethodCallExpr : _methodCallExpr
        {
            public _monthMethodCallExpr(__GeneratedOdata.CstNodes.Rules._monthMethodCallExpr _monthMethodCallExpr_1)
            {
                this._monthMethodCallExpr_1 = _monthMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._monthMethodCallExpr _monthMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _dayMethodCallExpr : _methodCallExpr
        {
            public _dayMethodCallExpr(__GeneratedOdata.CstNodes.Rules._dayMethodCallExpr _dayMethodCallExpr_1)
            {
                this._dayMethodCallExpr_1 = _dayMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._dayMethodCallExpr _dayMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _hourMethodCallExpr : _methodCallExpr
        {
            public _hourMethodCallExpr(__GeneratedOdata.CstNodes.Rules._hourMethodCallExpr _hourMethodCallExpr_1)
            {
                this._hourMethodCallExpr_1 = _hourMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._hourMethodCallExpr _hourMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _minuteMethodCallExpr : _methodCallExpr
        {
            public _minuteMethodCallExpr(__GeneratedOdata.CstNodes.Rules._minuteMethodCallExpr _minuteMethodCallExpr_1)
            {
                this._minuteMethodCallExpr_1 = _minuteMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._minuteMethodCallExpr _minuteMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _secondMethodCallExpr : _methodCallExpr
        {
            public _secondMethodCallExpr(__GeneratedOdata.CstNodes.Rules._secondMethodCallExpr _secondMethodCallExpr_1)
            {
                this._secondMethodCallExpr_1 = _secondMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._secondMethodCallExpr _secondMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _fractionalsecondsMethodCallExpr : _methodCallExpr
        {
            public _fractionalsecondsMethodCallExpr(__GeneratedOdata.CstNodes.Rules._fractionalsecondsMethodCallExpr _fractionalsecondsMethodCallExpr_1)
            {
                this._fractionalsecondsMethodCallExpr_1 = _fractionalsecondsMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._fractionalsecondsMethodCallExpr _fractionalsecondsMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _totalsecondsMethodCallExpr : _methodCallExpr
        {
            public _totalsecondsMethodCallExpr(__GeneratedOdata.CstNodes.Rules._totalsecondsMethodCallExpr _totalsecondsMethodCallExpr_1)
            {
                this._totalsecondsMethodCallExpr_1 = _totalsecondsMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._totalsecondsMethodCallExpr _totalsecondsMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _dateMethodCallExpr : _methodCallExpr
        {
            public _dateMethodCallExpr(__GeneratedOdata.CstNodes.Rules._dateMethodCallExpr _dateMethodCallExpr_1)
            {
                this._dateMethodCallExpr_1 = _dateMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._dateMethodCallExpr _dateMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _timeMethodCallExpr : _methodCallExpr
        {
            public _timeMethodCallExpr(__GeneratedOdata.CstNodes.Rules._timeMethodCallExpr _timeMethodCallExpr_1)
            {
                this._timeMethodCallExpr_1 = _timeMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._timeMethodCallExpr _timeMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _roundMethodCallExpr : _methodCallExpr
        {
            public _roundMethodCallExpr(__GeneratedOdata.CstNodes.Rules._roundMethodCallExpr _roundMethodCallExpr_1)
            {
                this._roundMethodCallExpr_1 = _roundMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._roundMethodCallExpr _roundMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _floorMethodCallExpr : _methodCallExpr
        {
            public _floorMethodCallExpr(__GeneratedOdata.CstNodes.Rules._floorMethodCallExpr _floorMethodCallExpr_1)
            {
                this._floorMethodCallExpr_1 = _floorMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._floorMethodCallExpr _floorMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ceilingMethodCallExpr : _methodCallExpr
        {
            public _ceilingMethodCallExpr(__GeneratedOdata.CstNodes.Rules._ceilingMethodCallExpr _ceilingMethodCallExpr_1)
            {
                this._ceilingMethodCallExpr_1 = _ceilingMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._ceilingMethodCallExpr _ceilingMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _distanceMethodCallExpr : _methodCallExpr
        {
            public _distanceMethodCallExpr(__GeneratedOdata.CstNodes.Rules._distanceMethodCallExpr _distanceMethodCallExpr_1)
            {
                this._distanceMethodCallExpr_1 = _distanceMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._distanceMethodCallExpr _distanceMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geoLengthMethodCallExpr : _methodCallExpr
        {
            public _geoLengthMethodCallExpr(__GeneratedOdata.CstNodes.Rules._geoLengthMethodCallExpr _geoLengthMethodCallExpr_1)
            {
                this._geoLengthMethodCallExpr_1 = _geoLengthMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._geoLengthMethodCallExpr _geoLengthMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _totalOffsetMinutesMethodCallExpr : _methodCallExpr
        {
            public _totalOffsetMinutesMethodCallExpr(__GeneratedOdata.CstNodes.Rules._totalOffsetMinutesMethodCallExpr _totalOffsetMinutesMethodCallExpr_1)
            {
                this._totalOffsetMinutesMethodCallExpr_1 = _totalOffsetMinutesMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._totalOffsetMinutesMethodCallExpr _totalOffsetMinutesMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _minDateTimeMethodCallExpr : _methodCallExpr
        {
            public _minDateTimeMethodCallExpr(__GeneratedOdata.CstNodes.Rules._minDateTimeMethodCallExpr _minDateTimeMethodCallExpr_1)
            {
                this._minDateTimeMethodCallExpr_1 = _minDateTimeMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._minDateTimeMethodCallExpr _minDateTimeMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _maxDateTimeMethodCallExpr : _methodCallExpr
        {
            public _maxDateTimeMethodCallExpr(__GeneratedOdata.CstNodes.Rules._maxDateTimeMethodCallExpr _maxDateTimeMethodCallExpr_1)
            {
                this._maxDateTimeMethodCallExpr_1 = _maxDateTimeMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._maxDateTimeMethodCallExpr _maxDateTimeMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _nowMethodCallExpr : _methodCallExpr
        {
            public _nowMethodCallExpr(__GeneratedOdata.CstNodes.Rules._nowMethodCallExpr _nowMethodCallExpr_1)
            {
                this._nowMethodCallExpr_1 = _nowMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._nowMethodCallExpr _nowMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _boolMethodCallExpr : _methodCallExpr
        {
            public _boolMethodCallExpr(__GeneratedOdata.CstNodes.Rules._boolMethodCallExpr _boolMethodCallExpr_1)
            {
                this._boolMethodCallExpr_1 = _boolMethodCallExpr_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr _boolMethodCallExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}