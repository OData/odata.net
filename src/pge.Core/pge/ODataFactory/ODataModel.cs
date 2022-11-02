namespace pge.ODataFactory
{
    using System;

    public sealed class ODataModel
    {
        public ODataModel(string model, ODataModelFormat format)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            this.Model = model;
            this.Format = format;
        }

        public string Model { get; }

        public ODataModelFormat Format { get; }
    }
}
