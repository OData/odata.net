//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Client
{
    /// <summary>
    /// options when saving changes
    /// </summary>
    [Flags]
    public enum SaveChangesOptions
    {
        /// <summary>default option, using multiple requests to the server stopping on the first failure</summary>
        None = 0,

        /// <summary>save the changes in a single changeset in a batch request.</summary>
        Batch = 1,

        /// <summary>save all the changes using multiple requests</summary>
        ContinueOnError = 2,

        /// <summary>Use replace semantics when doing update.</summary>
        ReplaceOnUpdate = 4,

        /// <summary>Use PATCH verb when doing update (retains the merge semantics).</summary>
        PatchOnUpdate = 8,

        /// <summary>save each change independently in a batch request.</summary>
        BatchWithIndependentOperations = 16,

        /// <summary> 
        /// Use partial payload when doing post. 
        /// Note it can only be used when using <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" /> 
        /// </summary> 
        PostOnlySetProperties = 32,
    }
}

