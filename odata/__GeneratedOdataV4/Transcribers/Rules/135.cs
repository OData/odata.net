namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _functionExprParameterTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._functionExprParameter>
    {
        private _functionExprParameterTranscriber()
        {
        }
        
        public static _functionExprParameterTranscriber Instance { get; } = new _functionExprParameterTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._functionExprParameter value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._parameterNameTranscriber.Instance.Transcribe(value._parameterName_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ⲤparameterAliasⳆparameterValueↃTranscriber.Instance.Transcribe(value._ⲤparameterAliasⳆparameterValueↃ_1, builder);

        }
    }
    
}
