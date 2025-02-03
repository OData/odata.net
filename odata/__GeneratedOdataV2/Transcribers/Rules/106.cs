namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _contextFragmentTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._contextFragment>
    {
        private _contextFragmentTranscriber()
        {
        }
        
        public static _contextFragmentTranscriber Instance { get; } = new _contextFragmentTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._contextFragment value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._contextFragment.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺTranscriber.Instance.Transcribe(node._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x24x72x65x66x29ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._contextFragment._ʺx24x72x65x66ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx24x72x65x66ʺTranscriber.Instance.Transcribe(node._ʺx24x72x65x66ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺTranscriber.Instance.Transcribe(node._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex45x6Ex74x69x74x79x54x79x70x65x29ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._contextFragment._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺTranscriber.Instance.Transcribe(node._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28x45x64x6Dx2Ex43x6Fx6Dx70x6Cx65x78x54x79x70x65x29ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._contextFragment._singletonEntity_꘡navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡꘡_꘡selectList꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._singletonEntityTranscriber.Instance.Transcribe(node._singletonEntity_1, context);
if (node._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡_1 != null)
{
__GeneratedOdataV2.Trancsribers.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Transcriber.Instance.Transcribe(node._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡_1, context);
}
if (node._selectList_1 != null)
{
__GeneratedOdataV2.Trancsribers.Rules._selectListTranscriber.Instance.Transcribe(node._selectList_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._contextFragment._qualifiedTypeName_꘡selectList꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._qualifiedTypeNameTranscriber.Instance.Transcribe(node._qualifiedTypeName_1, context);
if (node._selectList_1 != null)
{
__GeneratedOdataV2.Trancsribers.Rules._selectListTranscriber.Instance.Transcribe(node._selectList_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._contextFragment._entitySet_Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._entitySetTranscriber.Instance.Transcribe(node._entitySet_1, context);
__GeneratedOdataV2.Trancsribers.Inners._Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃTranscriber.Instance.Transcribe(node._Ⲥʺx2Fx24x64x65x6Cx65x74x65x64x45x6Ex74x69x74x79ʺⳆʺx2Fx24x6Cx69x6Ex6BʺⳆʺx2Fx24x64x65x6Cx65x74x65x64x4Cx69x6Ex6BʺↃ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._contextFragment._entitySet_keyPredicate_ʺx2Fʺ_contextPropertyPath_꘡selectList꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._entitySetTranscriber.Instance.Transcribe(node._entitySet_1, context);
__GeneratedOdataV2.Trancsribers.Rules._keyPredicateTranscriber.Instance.Transcribe(node._keyPredicate_1, context);
__GeneratedOdataV2.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);
__GeneratedOdataV2.Trancsribers.Rules._contextPropertyPathTranscriber.Instance.Transcribe(node._contextPropertyPath_1, context);
if (node._selectList_1 != null)
{
__GeneratedOdataV2.Trancsribers.Rules._selectListTranscriber.Instance.Transcribe(node._selectList_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._contextFragment._entitySet_꘡selectList꘡_꘡ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._entitySetTranscriber.Instance.Transcribe(node._entitySet_1, context);
if (node._selectList_1 != null)
{
__GeneratedOdataV2.Trancsribers.Rules._selectListTranscriber.Instance.Transcribe(node._selectList_1, context);
}
if (node._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ_1 != null)
{
__GeneratedOdataV2.Trancsribers.Inners._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺTranscriber.Instance.Transcribe(node._ʺx2Fx24x65x6Ex74x69x74x79ʺⳆʺx2Fx24x64x65x6Cx74x61ʺ_1, context);
}

return default;
            }
        }
    }
    
}
