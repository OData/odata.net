//---------------------------------------------------------------------
// <copyright file="ModelException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;		    //Reflection
using System.Collections.Generic;	//List<T>
using System.Threading;			    //Timeout

namespace Microsoft.Test.KoKoMo
{
	////////////////////////////////////////////////////////////////
	// ModelException
	//
	////////////////////////////////////////////////////////////////
	/// <summary>
	/// Any exception thrown from KoKoMo code.
	/// </summary>
	public class ModelException : Exception
	{
		//Constructors
		public ModelException(ModelItem item, string message)
			: this(item, message, null, null)
		{
			//Delegate
		}

		public ModelException(ModelItem item, string message, Exception inner)
			: this(item, message, inner, null)
		{
			//Delegate
		}

		public ModelException(ModelEngine engine, string message)
			: this(null, "Engine: " + message, null, engine.ActionsTrace)
		{
			//Delegate
		}

		public ModelException(ModelEngine engine, string message, Exception inner)
			: this(null, "Engine: " + message, inner, engine.ActionsTrace)
		{
			//Delegate
		}

        public ModelException(ModelItem source, string message, Exception inner, List<ModelActionInfo>   actions)
			: base(source != null ? (source.FullName + ": " + message) : message, inner)
		{
		}
	}
}
