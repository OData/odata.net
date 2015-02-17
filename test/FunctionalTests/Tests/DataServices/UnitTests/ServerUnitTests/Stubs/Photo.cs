//---------------------------------------------------------------------
// <copyright file="Photo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    #region Namespaces

    using System;
    using Microsoft.OData.Client;

    #endregion Namespaces
    [HasStream]
    //    [ETag(/*"Name", "Description",*/ "Rating", "ThumbNail", "LastUpdated")]
    public class Photo : Item, IEquatable<Photo>
    {
        public Photo()
        {
            ReInit();
        }

        public int Rating { get; set; }
        public byte[] ThumbNail { get; set; }

        public override void ReInit()
        {
            base.ReInit();
            Rating = 0;
            ThumbNail = null;
        }

        #region IEquatable<Photo> Members

        public bool Equals(Photo other)
        {
            return this.ID == other.ID;
        }

        #endregion
    }
}
