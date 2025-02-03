namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _collectionNavPathTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._collectionNavPath>
    {
        private _collectionNavPathTranscriber()
        {
        }
        
        public static _collectionNavPathTranscriber Instance { get; } = new _collectionNavPathTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._collectionNavPath value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._collectionNavPath.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._keyPredicateTranscriber.Instance.Transcribe(node._keyPredicate_1, context);
if (node._singleNavigation_1 != null)
{
__GeneratedOdataV2.Trancsribers.Rules._singleNavigationTranscriber.Instance.Transcribe(node._singleNavigation_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._collectionNavPath._filterInPath_꘡collectionNavigation꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._filterInPathTranscriber.Instance.Transcribe(node._filterInPath_1, context);
if (node._collectionNavigation_1 != null)
{
__GeneratedOdataV2.Trancsribers.Rules._collectionNavigationTranscriber.Instance.Transcribe(node._collectionNavigation_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._collectionNavPath._each_꘡boundOperation꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._eachTranscriber.Instance.Transcribe(node._each_1, context);
if (node._boundOperation_1 != null)
{
__GeneratedOdataV2.Trancsribers.Rules._boundOperationTranscriber.Instance.Transcribe(node._boundOperation_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._collectionNavPath._boundOperation node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._boundOperationTranscriber.Instance.Transcribe(node._boundOperation_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._collectionNavPath._count node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._countTranscriber.Instance.Transcribe(node._count_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._collectionNavPath._ref node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._refTranscriber.Instance.Transcribe(node._ref_1, context);

return default;
            }
        }
    }
    
}
