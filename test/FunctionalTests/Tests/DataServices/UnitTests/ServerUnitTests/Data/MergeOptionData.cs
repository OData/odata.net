//---------------------------------------------------------------------
// <copyright file="MergeOptionData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Data
{
    #region Namespaces

    using Microsoft.OData.Client;

    #endregion Namespaces

    public class MergeOptionData
    {
        private static MergeOptionData[] values;
        private readonly MergeOption option;

        private MergeOptionData(MergeOption option)
        {
            this.option = option;
        }

        public static MergeOptionData[] Values
        {
            get
            {
                if (values == null)
                {
                    values = new MergeOptionData[]
                        {
                            new MergeOptionData(MergeOption.AppendOnly),
                            new MergeOptionData(MergeOption.NoTracking),
                            new MergeOptionData(MergeOption.OverwriteChanges),
                            new MergeOptionData(MergeOption.PreserveChanges),
                        };
                }

                return values;
            }
        }

        public bool IsTracking
        {
            get { return this.option != MergeOption.NoTracking; }
        }

        public MergeOption Option
        {
            get { return this.option; }
        }

        public override string ToString()
        {
            return this.option.ToString();
        }
    }
}
