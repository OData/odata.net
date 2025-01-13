namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _functionParameterTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._functionParameter>
    {
        private _functionParameterTranscriber()
        {
        }
        
        public static _functionParameterTranscriber Instance { get; } = new _functionParameterTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._functionParameter value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._parameterNameTranscriber.Instance.Transcribe(value._parameterName_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdata.Trancsribers.Inners._ⲤparameterAliasⳆprimitiveLiteralↃTranscriber.Instance.Transcribe(value._ⲤparameterAliasⳆprimitiveLiteralↃ_1, builder);

        }
    }
    
}
