namespace NewStuff._Design._0_Convention.Readers
{
	public sealed class HeaderFieldValue
	{
		internal HeaderFieldValue(string value)
		{
			Value = value;
		}

		internal string Value { get; }
	}
}
