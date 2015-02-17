//---------------------------------------------------------------------
// <copyright file="Util.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;		//ArrayList
using System.Diagnostics;		//Assert

namespace Microsoft.Test.ModuleCore
{
	////////////////////////////////////////////////////////////////
	// StringEx
	//
	////////////////////////////////////////////////////////////////
	internal class StringEx
	{
		static public string ToString(object value)
		{
			if(value == null)
				return null;

			if(value.GetType().IsEnum)
			{
				//Note: Value is not quite as easy as simply calling ToString().  Enums for example
				//will return the type of the enum object and not the string of value...

				// Breaking change 628 (URT >= 2218): Format is being removed. 
				// e.ToString("G") is the replacement for format. 
				//return ((Enum)value).Format();
				return ((Enum)value).ToString("G");
			}

			return value.ToString();
		}

		static public string Format(object value)
		{
			if(value == null)
				return "(null)";
			if(value is string)
				return "\"" + value + "\"";

			return ToString(value);
		}
	}

	////////////////////////////////////////////////////////////////
	// TypeEx
	//
	////////////////////////////////////////////////////////////////
    internal class TypeEx
    {
        public static ArrayList    GetNestedTypes(Type type, bool inherit)
        {
            //GetNestedTypes: 
            //  "This method returns only the nested types of the current type. 
            //   It does not search the hierarchy of inherited types. 
            //   To find types that are nested in inherited types, you must walk the inheritance hierarchy."
            ArrayList types = new ArrayList();
            
            //Recurse
            if(inherit && type.BaseType != null)
                types = GetNestedTypes(type.BaseType, inherit);

            //Nested Types
            foreach(Type t in type.GetNestedTypes())
                types.Add(t);

            return types;
        }
    }

	////////////////////////////////////////////////////////////////
	// InsensitiveHashtable
	//
	////////////////////////////////////////////////////////////////
	internal class InsensitiveHashtable : Hashtable
	{
		//Case-insensitive
		public InsensitiveHashtable(int capacity)
			: base(capacity, StringComparer.InvariantCultureIgnoreCase)
		{
		}

		//Helpers
		public	void	Update(object key, object value)
		{
			//If it already exist, update the value
			if(Contains(key))
			{
				this[key] = value;
				return;
			}

			//Otheriwse add the value
			Add(key, value);
		}

		public	void	Dump()
		{
			//Loop over all keys
			foreach(object key in Keys)
				Console.WriteLine(key + "=" + this[key] + ";");
		}
	}
}
