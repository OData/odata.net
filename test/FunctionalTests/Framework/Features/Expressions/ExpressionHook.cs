//---------------------------------------------------------------------
// <copyright file="ExpressionHook.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

public partial class DataServiceClass
{
    public DataServiceClass() : base()
    {
        // Hook up to requestQueryableConstructed
        FieldInfo fi = typeof(DataService<ContextTypeName>).GetField("requestQueryableConstructed", BindingFlags.Instance | BindingFlags.SetField | BindingFlags.NonPublic);

        if (fi != null)
        {
            // Call the trusted method to handle Partial trust testing.
            System.Data.Test.Astoria.FullTrust.TrustedMethods.FieldInfoSetValue(fi, this, new Action<IQueryable>(OnQueryableConstructed));
        }
    }

    // Last seen IQueryable.
    private static IQueryable lastQueryable;
    private void OnQueryableConstructed(IQueryable result) { lastQueryable = result; }

    // Service operation to retrieve XML expression tree.
    [WebGet]
    [SingleResult]
    public IQueryable<String> ExpToXml()
    {
        string[] x = new string[]
        {
            lastQueryable != null
                ? lastQueryable.Expression.ToString() + "[[===]]" + AstoriaUnitTests.Stubs.ExpressionTreeToXmlSerializer.SerializeToString(lastQueryable.Expression)
                : "WARNING: last IQueryable not set, make sure product build is checked"
        };
        lastQueryable = null;
        return x.AsQueryable();
    }
}
