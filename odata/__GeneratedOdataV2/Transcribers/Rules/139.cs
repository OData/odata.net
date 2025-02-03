namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _methodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._methodCallExpr>
    {
        private _methodCallExprTranscriber()
        {
        }
        
        public static _methodCallExprTranscriber Instance { get; } = new _methodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._methodCallExpr.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._indexOfMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._indexOfMethodCallExprTranscriber.Instance.Transcribe(node._indexOfMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._toLowerMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._toLowerMethodCallExprTranscriber.Instance.Transcribe(node._toLowerMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._toUpperMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._toUpperMethodCallExprTranscriber.Instance.Transcribe(node._toUpperMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._trimMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._trimMethodCallExprTranscriber.Instance.Transcribe(node._trimMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._substringMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._substringMethodCallExprTranscriber.Instance.Transcribe(node._substringMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._concatMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._concatMethodCallExprTranscriber.Instance.Transcribe(node._concatMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._lengthMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._lengthMethodCallExprTranscriber.Instance.Transcribe(node._lengthMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._yearMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._yearMethodCallExprTranscriber.Instance.Transcribe(node._yearMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._monthMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._monthMethodCallExprTranscriber.Instance.Transcribe(node._monthMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._dayMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._dayMethodCallExprTranscriber.Instance.Transcribe(node._dayMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._hourMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._hourMethodCallExprTranscriber.Instance.Transcribe(node._hourMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._minuteMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._minuteMethodCallExprTranscriber.Instance.Transcribe(node._minuteMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._secondMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._secondMethodCallExprTranscriber.Instance.Transcribe(node._secondMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._fractionalsecondsMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._fractionalsecondsMethodCallExprTranscriber.Instance.Transcribe(node._fractionalsecondsMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._totalsecondsMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._totalsecondsMethodCallExprTranscriber.Instance.Transcribe(node._totalsecondsMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._dateMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._dateMethodCallExprTranscriber.Instance.Transcribe(node._dateMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._timeMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._timeMethodCallExprTranscriber.Instance.Transcribe(node._timeMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._roundMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._roundMethodCallExprTranscriber.Instance.Transcribe(node._roundMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._floorMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._floorMethodCallExprTranscriber.Instance.Transcribe(node._floorMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._ceilingMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._ceilingMethodCallExprTranscriber.Instance.Transcribe(node._ceilingMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._distanceMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._distanceMethodCallExprTranscriber.Instance.Transcribe(node._distanceMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._geoLengthMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._geoLengthMethodCallExprTranscriber.Instance.Transcribe(node._geoLengthMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._totalOffsetMinutesMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._totalOffsetMinutesMethodCallExprTranscriber.Instance.Transcribe(node._totalOffsetMinutesMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._minDateTimeMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._minDateTimeMethodCallExprTranscriber.Instance.Transcribe(node._minDateTimeMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._maxDateTimeMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._maxDateTimeMethodCallExprTranscriber.Instance.Transcribe(node._maxDateTimeMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._nowMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._nowMethodCallExprTranscriber.Instance.Transcribe(node._nowMethodCallExpr_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._methodCallExpr._boolMethodCallExpr node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._boolMethodCallExprTranscriber.Instance.Transcribe(node._boolMethodCallExpr_1, context);

return default;
            }
        }
    }
    
}
