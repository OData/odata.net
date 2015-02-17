//---------------------------------------------------------------------
// <copyright file="PicturesTagsClientPOCO.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using Microsoft.OData.Client;

namespace PTClientPOCO {

    public partial class PTEntities : DataServiceContext {
        public PTEntities(global::System.Uri serviceRoot) : base(serviceRoot) {
            this.ResolveName = this.ResolveNameFromType;
            this.ResolveType = this.ResolveTypeFromName;
            this.OnContextCreated();
        }

        partial void OnContextCreated();
        
        protected Type ResolveTypeFromName(string typeName) {
            if (typeName.StartsWith("PT", StringComparison.Ordinal)) {
                return this.GetType().Assembly.GetType(string.Concat("PTClientPOCO", typeName.Substring(2)), true);
            }
            return null;
        }

        protected string ResolveNameFromType(global::System.Type clientType) {
            if (clientType.Namespace.Equals("PTClientPOCO", global::System.StringComparison.Ordinal)) {
                return string.Concat("PT.", clientType.Name);
            }
            return clientType.FullName;
        }
        
        public DataServiceQuery<Picture> Pictures {
            get { return base.CreateQuery<Picture>("Pictures");  }
        }

        public DataServiceQuery<Tag> Tags {
            get { return base.CreateQuery<Tag>("Tags"); }
        }

        public void AddToPictures(Picture picture) {
            base.AddObject("Pictures", picture);
        }

        public void AddToTags(Tag tag) {
            base.AddObject("Tags", tag);
        }
    }

    [KeyAttribute("Id")]
    public partial class Picture {
        public Picture() {
            Tags = new Collection<Tag>();
        }
        
        public static Picture CreatePicture(int ID) {
            Picture picture = new Picture();
            picture.Id = ID;
            return picture;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public Collection<Tag> Tags { get; set;}
    }

    [KeyAttribute("Id")]
    public partial class Tag {
        public Tag() {
            Pictures = new Collection<Picture>();
        }

        public static Tag CreateTag(int ID, string tagName) {
            Tag tag = new Tag();
            tag.Id = ID;
            tag.TagName = tagName;
            return tag;
        }

        public int Id { get; set; }
        public string TagName { get; set;}
        public Collection<Picture> Pictures { get; set;}
    }

    [KeyAttribute("Id")]
    public partial class InternalPicture : Picture {

        public static InternalPicture CreateInternalPicture(int ID) {
            InternalPicture internalPicture = new InternalPicture();
            internalPicture.Id = ID;
            return internalPicture;
        }

        public string Exif { get; set; }
        public FileStorage FileStorage { get; set; }
    }

    [KeyAttribute("Id")]
    public partial class FileStorage {
        public static FileStorage CreateFileStorage(int ID, string location) {
            FileStorage fileStorage = new FileStorage();
            fileStorage.Id = ID;
            fileStorage.Location = location;
            return fileStorage;
        }

        public int Id { get; set; }
        public string Location { get; set; }
        public string ContentType { get; set;}
        public string ContentDisposition { get; set; }
        public InternalPicture Picture { get; set; }
    }
    
    [KeyAttribute("Id")]
    public partial class DummyPictureNonMLE : Picture {
        public static DummyPictureNonMLE CreateDummyPictureNonMLE(int ID) {
            DummyPictureNonMLE dummyPictureNonMLE = new DummyPictureNonMLE();
            dummyPictureNonMLE.Id = ID;
            return dummyPictureNonMLE;
        }

        public string DummyMessage { get; set; }
    }

    [KeyAttribute("Id")]
    public partial class ExternalPicture : Picture {

        public static ExternalPicture CreateExternalPicture(int ID, string uRL) {
            ExternalPicture externalPicture = new ExternalPicture();
            externalPicture.Id = ID;
            externalPicture.URL = uRL;
            return externalPicture;
        }

        public string URL { get; set; }
        public string ContentType { get; set; }
        public string BlobETag { get; set; }
    }
}
