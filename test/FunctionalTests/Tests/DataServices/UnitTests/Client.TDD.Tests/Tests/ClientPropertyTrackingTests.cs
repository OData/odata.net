//---------------------------------------------------------------------
// <copyright file="ClientPropertyTrackingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [Key("ID")]
    public class EntityType : INotifyPropertyChanged
    {
        #region ID
        public int ID
        {
            get { return this._ID; }
            set { this._ID = value; this.OnPropertyChanged("ID"); }
        }
        private int _ID;
        #endregion

        #region StringCollection
        public ObservableCollection<string> StringCollection
        {
            get { return this._StringCollection; }
            set
            {
                this._StringCollection = value;
                this.OnPropertyChanged("StringCollection");
            }
        }
        private ObservableCollection<string> _StringCollection = new ObservableCollection<string>();
        #endregion

        #region Complex
        public ComplexType Complex
        {
            get { return this._Complex; }
            set
            {
                this._Complex = value;
                this.OnPropertyChanged("Complex");
            }
        }
        private ComplexType _Complex;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class ComplexType : INotifyPropertyChanged
    {
        #region Name
        public string Name
        {
            get { return this._Name; }
            set { this._Name = value; this.OnPropertyChanged("Name"); }
        }
        private string _Name;
        #endregion

        #region Home
        public Address Home
        {
            get { return this._Home; }
            set
            {
                this._Home = value;
                this.OnPropertyChanged("Home");
            }
        }
        private Address _Home;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Address : INotifyPropertyChanged
    {
        public Address(string street, int code)
        {
            this._Street = street;
            this._Code = code;
        }

        #region Street
        public string Street
        {
            get { return this._Street; }
            set { this._Street = value; this.OnPropertyChanged("Street"); }
        }
        private string _Street;
        #endregion

        #region Code
        public int Code
        {
            get { return this._Code; }
            set
            {
                this._Code = value;
                this.OnPropertyChanged("Code");
            }
        }
        private int _Code;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class HomeAddress : Address
    {
        public HomeAddress(string street, int code, string city)
            : base(street, code)
        {
            this._City = city;
        }

        #region City
        public string City
        {
            get { return this._City; }
            set { this._City = value; this.OnPropertyChanged("City"); }
        }
        private string _City;

        #endregion
    }

    [Key("ID")]
    public class DerivedEntityType : EntityType
    {
        #region Name
        public string Name
        {
            get { return this._Name; }
            set { this._Name = value; this.OnPropertyChanged("Name"); }
        }
        private string _Name;
        #endregion

        #region Home
        public HomeAddress Home
        {
            get { return this._Home; }
            set { this._Home = value; this.OnPropertyChanged("Home"); }
        }
        private HomeAddress _Home;

        #endregion
    }

    public abstract class Component<T>
    { }

    [Key("ID")]
    public class Asset : Component<Asset>
    {
        public int ID { get; set; }
    }

    [TestClass]
    public class ClientPropertyTrackingTests
    {
        private DataServiceContext context;

        [TestInitialize]
        public void Init()
        {
            this.context = new DataServiceContext(new Uri("http://www.odata.org/service.svc"));
        }

        [TestMethod]
        public void UpdatePrimitivePropertyShouldOnlySerializeChangedProperties()
        {
            EntityType entity = new EntityType
            {
                ID = 1,
                Complex = new ComplexType
                {
                    Name = "June",
                    Home = new Address("Dongchuan", 200)
                },
                StringCollection = new ObservableCollection<string>(new string[] { "first", "second" })
            };
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, null, TrackingMode.AutoChangeTracking, "EntitySet", null, null);
            collection.Add(entity);

            context.Entities.FirstOrDefault().State = EntityStates.Unchanged;
            entity.ID = 2;
            Assert.AreEqual(EntityStates.Modified, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"ID\":2}");
        }

        [TestMethod]
        public void UpdateCollectionPropertyShouldOnlySerializeChangedProperties()
        {
            EntityType entity = new EntityType
            {
                ID = 1,
                Complex = new ComplexType
                {
                    Name = "June",
                    Home = new Address("Dongchuan", 200)
                },
                StringCollection = new ObservableCollection<string>(new string[] { "first", "second" })
            };
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, null, TrackingMode.AutoChangeTracking, "EntitySet", null, null);
            collection.Add(entity);

            context.Entities.FirstOrDefault().State = EntityStates.Unchanged;
            entity.StringCollection.Add("third");
            Assert.AreEqual(EntityStates.Modified, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"StringCollection@odata.type\":\"#Collection(String)\",\"StringCollection\":[\"first\",\"second\",\"third\"]}");
        }

        [TestMethod]
        public void UpdateComplexPropertyShouldOnlySerializeChangedProperties()
        {
            EntityType entity = new EntityType
            {
                ID = 1,
                Complex = new ComplexType
                {
                    Name = "June",
                    Home = new Address("Dongchuan", 200)
                },
                StringCollection = new ObservableCollection<string>(new string[] { "first", "second" })
            };
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, null, TrackingMode.AutoChangeTracking, "EntitySet", null, null);
            collection.Add(entity);

            context.Entities.FirstOrDefault().State = EntityStates.Unchanged;
            entity.Complex = new ComplexType
            {
                Name = "July",
                Home = new Address("Jiaotong", 16)
            };
            Assert.AreEqual(EntityStates.Modified, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"Complex\":{\"Name\":\"July\",\"Home\":{\"Code\":16,\"Street\":\"Jiaotong\"}}}");
        }

        [TestMethod]
        public void UpdatePropertyInComplexPropertyShouldOnlySerializeChangedProperties()
        {
            EntityType entity = new EntityType
            {
                ID = 1,
                Complex = new ComplexType
                {
                    Name = "June",
                    Home = new Address("Dongchuan", 200)
                },
                StringCollection = new ObservableCollection<string>(new string[] { "first", "second" })
            };
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, null, TrackingMode.AutoChangeTracking, "EntitySet", null, null);
            collection.Add(entity);

            context.Entities.FirstOrDefault().State = EntityStates.Unchanged;
            entity.Complex.Name = "July";
            Assert.AreEqual(EntityStates.Modified, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"Complex\":{\"Name\":\"July\",\"Home\":{\"Code\":200,\"Street\":\"Dongchuan\"}}}");
        }

        [TestMethod]
        public void UpdateComplexPropertyInComplexPropertyShouldOnlySerializeChangedProperties()
        {
            EntityType entity = new EntityType
            {
                ID = 1,
                Complex = new ComplexType
                {
                    Name = "June",
                    Home = new Address("Dongchuan", 200)
                },
                StringCollection = new ObservableCollection<string>(new string[] { "first", "second" })
            };
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, null, TrackingMode.AutoChangeTracking, "EntitySet", null, null);
            collection.Add(entity);

            context.Entities.FirstOrDefault().State = EntityStates.Unchanged;
            entity.Complex.Home.Code = 17;
            Assert.AreEqual(EntityStates.Modified, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"Complex\":{\"Name\":\"June\",\"Home\":{\"Code\":17,\"Street\":\"Dongchuan\"}}}");
        }

        [TestMethod]
        public void UpdateMultiplePropertiesShouldOnlySerializeChangedProperties()
        {
            EntityType entity = new EntityType
            {
                ID = 1,
                Complex = new ComplexType
                {
                    Name = "June",
                    Home = new Address("Dongchuan", 200)
                },
                StringCollection = new ObservableCollection<string>(new string[] { "first", "second" })
            };
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, null, TrackingMode.AutoChangeTracking, "EntitySet", null, null);
            collection.Add(entity);

            context.Entities.FirstOrDefault().State = EntityStates.Unchanged;
            entity.ID = 71;
            entity.Complex.Home.Code = 17;
            Assert.AreEqual(EntityStates.Modified, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"ID\":71,\"Complex\":{\"Name\":\"June\",\"Home\":{\"Code\":17,\"Street\":\"Dongchuan\"}}}");
        }

        [TestMethod]
        public void UpdateDerivedEntityTypeShouldOnlySerializeChangedProperties()
        {
            DerivedEntityType entity = new DerivedEntityType
            {
                ID = 7,
                Complex = new ComplexType
                {
                    Name = "June",
                    Home = new Address("Xiadu", 71)
                },
                StringCollection = new ObservableCollection<string>(new string[] { "first", "second" }),
                Name = "July"
            };
            DataServiceCollection<DerivedEntityType> collection = new DataServiceCollection<DerivedEntityType>(context, null, TrackingMode.AutoChangeTracking, "EntitySet", null, null);
            collection.Add(entity);

            context.Entities.FirstOrDefault().State = EntityStates.Unchanged;
            entity.ID = 71;
            entity.Complex.Home.Code = 17;
            entity.Name = "Aug";
            Assert.AreEqual(EntityStates.Modified, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"ID\":71,\"Name\":\"Aug\",\"Complex\":{\"Name\":\"June\",\"Home\":{\"Code\":17,\"Street\":\"Xiadu\"}}}");
        }

        [TestMethod]
        public void UpdateDerivedComplexTypeShouldOnlySerializeChangedProperties()
        {
            DerivedEntityType entity = new DerivedEntityType
            {
                ID = 7,
                Complex = new ComplexType
                {
                    Name = "June",
                    Home = new Address("Xiadu", 71)
                },
                StringCollection = new ObservableCollection<string>(new string[] { "first", "second" }),
                Name = "July",
                Home = new HomeAddress("Xingang", 17, "Guangzhou")
            };
            DataServiceCollection<DerivedEntityType> collection = new DataServiceCollection<DerivedEntityType>(context, null, TrackingMode.AutoChangeTracking, "EntitySet", null, null);
            collection.Add(entity);

            context.Entities.FirstOrDefault().State = EntityStates.Unchanged;
            entity.ID = 71;
            entity.Complex.Home.Code = 17;
            entity.Home.City = "Guangzhou";
            Assert.AreEqual(EntityStates.Modified, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"ID\":71,\"Complex\":{\"Name\":\"June\",\"Home\":{\"Code\":17,\"Street\":\"Xiadu\"}},\"Home\":{\"City\":\"Guangzhou\",\"Code\":17,\"Street\":\"Xingang\"}}");
        }

        [TestMethod]
        public void DerivedEntityTypeShouldOnlySerializeChangedProperties()
        {
            EntityType entity = new DerivedEntityType
            {
                ID = 7,
                Complex = new ComplexType
                {
                    Name = "June",
                    Home = new Address("Xiadu", 71)
                },
                StringCollection = new ObservableCollection<string>(new string[] { "first", "second" }),
                Name = "July",
                Home = new HomeAddress("Xingang", 17, "Guangzhou")
            };
            DataServiceCollection<DerivedEntityType> collection = new DataServiceCollection<DerivedEntityType>(context, null, TrackingMode.AutoChangeTracking, "EntitySet", null, null);
            collection.Add((DerivedEntityType)entity);

            context.Entities.FirstOrDefault().State = EntityStates.Unchanged;
            entity.ID = 71;
            entity.Complex.Home.Code = 17;
            ((DerivedEntityType)entity).Home.City = "Guangzhou";
            Assert.AreEqual(EntityStates.Modified, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"ID\":71,\"Complex\":{\"Name\":\"June\",\"Home\":{\"Code\":17,\"Street\":\"Xiadu\"}},\"Home\":{\"City\":\"Guangzhou\",\"Code\":17,\"Street\":\"Xingang\"}}");
        }

        [TestMethod]
        public void DerivedComplexTypeShouldOnlySerializeChangedProperties()
        {
            EntityType entity = new EntityType
            {
                ID = 1,
                Complex = new ComplexType
                {
                    Name = "June",
                    Home = new HomeAddress("Xiadu", 200, "Guangzhou")
                },
                StringCollection = new ObservableCollection<string>(new string[] { "first", "second" })
            };
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, null, TrackingMode.AutoChangeTracking, "EntitySet", null, null);
            collection.Add(entity);

            context.Entities.FirstOrDefault().State = EntityStates.Unchanged;
            entity.ID = 71;
            entity.Complex.Home.Code = 17;
            ((HomeAddress)entity.Complex.Home).City = "Guangzhou";
            Assert.AreEqual(EntityStates.Modified, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"ID\":71,\"Complex\":{\"Name\":\"June\",\"Home\":{\"City\":\"Guangzhou\",\"Code\":17,\"Street\":\"Xiadu\"}}}");
        }

        [TestMethod]
        public void OnlySetPropertiesShouldBeSentOnPost()
        {
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, "EntitySet", null, null);
            EntityType entity = new EntityType();
            collection.Add(entity);
            entity.ID = 71;
            Assert.AreEqual(EntityStates.Added, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"ID\":71}", SaveChangesOptions.PostOnlySetProperties);
        }

        [TestMethod]
        public void OnlyChangedCollectionPropertiesShouldBeSentOnPost()
        {
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, "EntitySet", null, null);
            EntityType entity = new EntityType();
            collection.Add(entity);
            entity.StringCollection.Add("first");
            Assert.AreEqual(EntityStates.Added, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"StringCollection@odata.type\":\"#Collection(String)\",\"StringCollection\":[\"first\"]}", SaveChangesOptions.PostOnlySetProperties);
        }

        [TestMethod]
        public void OnlySetComplexPropertiesShouldBeSentOnPost()
        {
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, "EntitySet", null, null);
            EntityType entity = new EntityType();
            collection.Add(entity);
            entity.Complex = new ComplexType
            {
                Name = "July",
                Home = new Address("Jiaotong", 16)
            };
            Assert.AreEqual(EntityStates.Added, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"Complex\":{\"Name\":\"July\",\"Home\":{\"Code\":16,\"Street\":\"Jiaotong\"}}}", SaveChangesOptions.PostOnlySetProperties);
        }

        [TestMethod]
        public void OnlyChangedDerivedComplexTypePropertiesShouldBeSentOnPost()
        {
            DataServiceCollection<DerivedEntityType> collection = new DataServiceCollection<DerivedEntityType>(context, "EntitySet", null, null);
            DerivedEntityType entity = new DerivedEntityType();
            collection.Add(entity);
            entity.Home = new HomeAddress("Xiadu", 71, "Guangzhou");
            Assert.AreEqual(EntityStates.Added, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"Home\":{\"City\":\"Guangzhou\",\"Code\":71,\"Street\":\"Xiadu\"}}", SaveChangesOptions.PostOnlySetProperties);
        }

        [TestMethod]
        public void OnlyChangedPropertiesInDerivedEntityTypeShouldBeSentOnPost()
        {
            EntityType entity = new DerivedEntityType();
            DataServiceCollection<DerivedEntityType> collection = new DataServiceCollection<DerivedEntityType>(context, "EntitySet", null, null);
            collection.Add((DerivedEntityType)entity);
            entity.ID = 71;
            entity.Complex = new ComplexType
            {
                Name = "June",
                Home = new Address("Xiadu", 17)
            };
            ((DerivedEntityType)entity).Home = new HomeAddress("Xingang", 17, "Guangzhou");
            Assert.AreEqual(EntityStates.Added, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"ID\":71,\"Complex\":{\"Name\":\"June\",\"Home\":{\"Code\":17,\"Street\":\"Xiadu\"}},\"Home\":{\"City\":\"Guangzhou\",\"Code\":17,\"Street\":\"Xingang\"}}", SaveChangesOptions.PostOnlySetProperties);
        }

        [TestMethod]
        public void OnlyChangedPropertiesInEntityTypeAddedToDataServiceCollectionInitailizationShouldBeSentOnPost()
        {
            EntityType entity = new EntityType();
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, "EntitySet", null, null) { entity };
            entity.ID = 71;
            entity.Complex = new ComplexType
            {
                Name = "June",
                Home = new Address("Xiadu", 17)
            };
            Assert.AreEqual(EntityStates.Added, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"ID\":71,\"Complex\":{\"Name\":\"June\",\"Home\":{\"Code\":17,\"Street\":\"Xiadu\"}}}", SaveChangesOptions.PostOnlySetProperties);
        }

        [TestMethod]
        public void OnlyChangedPropertiesInEntityTypeInitializedInDataServiceCollectionShouldBeSentOnPost()
        {
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, "EntitySet", null, null) { new EntityType() };
            collection[0].ID = 71;
            collection[0].Complex = new ComplexType
            {
                Name = "June",
                Home = new Address("Xiadu", 17)
            };
            Assert.AreEqual(EntityStates.Added, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{\"ID\":71,\"Complex\":{\"Name\":\"June\",\"Home\":{\"Code\":17,\"Street\":\"Xiadu\"}}}", SaveChangesOptions.PostOnlySetProperties);
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void OnlyPostExplicitPropertiesUsedWithoutDataServiceCollectionShouldThrow()
        {
            EntityType entity = new EntityType { ID = 1 };
            context.AddObject("EntitySet", entity);
            Action action = () => context.SaveChanges(SaveChangesOptions.PostOnlySetProperties);
            action.ShouldThrow<InvalidOperationException>().WithMessage("'SaveChangesOptions.OnlyPostExplicitProperties' must be used with 'DataServiceCollection'.");
        }
#endif

        [TestMethod]
        public void TrackedEntityPayLoadShouldKeepEmptyIfNoPropertyHasBeenSetOnPost()
        {
            EntityType entity = new EntityType();
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, "EntitySet", null, null) { entity };
            Assert.AreEqual(EntityStates.Added, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{}", SaveChangesOptions.PostOnlySetProperties);
        }

        [TestMethod]
        public void TrackedEntityPayLoadShouldKeepEmptyIfNoPropertyHasBeenTrackedOnPost()
        {
            EntityType entity = new EntityType() { ID = 1 };
            DataServiceCollection<EntityType> collection = new DataServiceCollection<EntityType>(context, "EntitySet", null, null) { entity };
            Assert.AreEqual(EntityStates.Added, context.Entities.FirstOrDefault().State);
            ValidateBodyContent(context, "{}", SaveChangesOptions.PostOnlySetProperties);
        }

        [TestMethod]
        public void UnTrackedEntityPayLoadShouldBeFullPayLoadOnPost()
        {
            EntityType entity = new EntityType() { ID = 1 };
            context.AddObject("EntitySet", entity);
            ValidateBodyContent(context, "{\"ID\":1,\"StringCollection@odata.type\":\"#Collection(String)\",\"StringCollection\":[],\"Complex\":null}");
        }

        // [OData Client] Enable DataServiceCollection to handle entities which derives from a generic type in custom partial class
        [TestMethod]
        public void DataServiceCollectionCtorShouldNotThrowExceptionWhenParameterTDerivedFromAGenericTypeOfT()
        {
            var dsc = new DataServiceCollection<Asset>(context, null, TrackingMode.AutoChangeTracking, "EntitySet", null, null);
            Assert.IsNotNull(dsc);
        }

        private static void ValidateBodyContent(DataServiceContext ctx, string expectedContent, SaveChangesOptions options = SaveChangesOptions.None)
        {
            var sr = new TestBaseSaveResult(ctx, "SaveChanges", options, null, null);
            var s = sr.GetChangeStream().FirstOrDefault();

            s.Seek(0, SeekOrigin.Begin);
            StreamReader r = new StreamReader(s);
            String actual = r.ReadToEnd();

            actual.Should().Be(expectedContent);
        }

        internal class TestBaseSaveResult : SaveResult
        {
            public TestBaseSaveResult(DataServiceContext context, string method, SaveChangesOptions options, AsyncCallback callback, object state)
                : base(context, method, options, callback, state)
            {
            }

            internal List<Stream> GetChangeStream()
            {
                List<Stream> stream = new List<Stream>();
                HeaderCollection headers = new HeaderCollection();
                headers.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
                for (int i = 0; i < this.ChangedEntries.Count; ++i)
                {
                    ODataRequestMessageWrapper requestMessage = ODataRequestMessageWrapper.CreateRequestMessageWrapper(
                        new BuildingRequestEventArgs("GET", new Uri("http://service.svc/randomuri"), headers, null, HttpStack.Auto),
                        this.RequestInfo);
                    this.CreateChangeData(i, requestMessage);
                    stream.Add(requestMessage.CachedRequestStream.Stream);
                }

                return stream;
            }
        }
    }
}
