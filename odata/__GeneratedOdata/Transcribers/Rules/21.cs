namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _filterInPathTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._filterInPath>
    {
        private _filterInPathTranscriber()
        {
        }
        
        public static _filterInPathTranscriber Instance { get; } = new _filterInPathTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._filterInPath value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx2Fx24x66x69x6Cx74x65x72ʺTranscriber.Instance.Transcribe(value._ʺx2Fx24x66x69x6Cx74x65x72ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdata.Trancsribers.Rules._parameterAliasTranscriber.Instance.Transcribe(value._parameterAlias_1, builder);

        }
    }
    
}
