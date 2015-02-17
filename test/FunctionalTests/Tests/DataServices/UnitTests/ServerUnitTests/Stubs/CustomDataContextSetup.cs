//---------------------------------------------------------------------
// <copyright file="CustomDataContextSetup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    using System;
    using AstoriaUnitTests.Tests;
    using System.Diagnostics;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class CustomDataContextSetup
    {
        private Type dataServiceType;
        private Type entityType;
        private EventHandler handler;
        private EventInfo valuesRequestedEvent;
        private object memberValue;
        private object id;
        private object secondId;
        private PropertyInfo memberPropertyInfo;
        private PropertyInfo idPropertyInfo;
        private PropertyInfo secondIdPropertyInfo;

        public CustomDataContextSetup(Type entityType)
        {
            Debug.Assert(entityType != null);

            this.entityType = entityType;
            this.dataServiceType = typeof(TypedCustomDataContext<>).MakeGenericType(entityType);

            this.handler = new EventHandler(TypedDataContextValuesRequested);
            this.valuesRequestedEvent = this.dataServiceType.GetEvent("ValuesRequested", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(valuesRequestedEvent, "valuesRequestedEvent is not null");
            valuesRequestedEvent.AddEventHandler(this, handler);

            memberPropertyInfo = entityType.GetProperty("Member");
            if (entityType.GetGenericTypeDefinition() == typeof(TypedEntity<,>))
            {
                idPropertyInfo = entityType.GetProperty("ID");
            }
            else
            {
                Debug.Assert(entityType.GetGenericTypeDefinition() == typeof(DoubleKeyTypedEntity<,,>));
                idPropertyInfo = entityType.GetProperty("FirstKey");
                secondIdPropertyInfo = entityType.GetProperty("SecondKey");
            }
        }

        public void Cleanup()
        {
            valuesRequestedEvent.RemoveEventHandler(this, handler);
        }

        public Type DataServiceType
        {
            get { return this.dataServiceType; }
        }

        public object Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public object SecondId
        {
            get { return this.secondId; }
            set { this.secondId = value; }
        }

        public object MemberValue
        {
            get { return this.memberValue; }
            set { this.memberValue = value; }
        }

        /// <summary>
        /// Handles the ValuesRequested event on the data provider by
        /// filling values on demand.
        /// </summary>
        private void TypedDataContextValuesRequested(object sender, EventArgs e)
        {
            // Create a single item for the "entity set".
            object entity = Activator.CreateInstance(entityType);
            entityType.GetProperty("Member").SetValue(entity, memberValue, null);

            if (idPropertyInfo != null)
            {
                idPropertyInfo.SetValue(entity, this.id, null);
            }

            if (secondIdPropertyInfo != null)
            {
                secondIdPropertyInfo.SetValue(entity, this.secondId, null);
            }

            object[] parameters = new object[] { new object[] { entity } };
            sender.GetType().GetMethod("SetValues").Invoke(sender, parameters);
        }
    }
}
