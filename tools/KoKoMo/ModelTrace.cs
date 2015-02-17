//---------------------------------------------------------------------
// <copyright file="ModelTrace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;				
using System.IO;                //TextWriter
using System.Text;				//StringBuilder


namespace Microsoft.Test.KoKoMo
{
	////////////////////////////////////////////////////////////////
	// ModelTraceLevel
	//
	////////////////////////////////////////////////////////////////
	public enum ModelTraceLevel
	{
		//TODO:
	};

	////////////////////////////////////////////////////////////////
	// ModelTrace
	//
	////////////////////////////////////////////////////////////////
	public class ModelTrace
	{
		//Data
        static TextWriter   _writer     = Console.Out;

		//Accessors
		public static bool		    Enabled
		{
            //Note: This changes the default for all engines
            //You can also change this on a per engine basis:
            //  ModelEngine.Options.Tracing=true|false
			get { return ModelEngineOptions.Default.Tracing;	}
			set { ModelEngineOptions.Default.Tracing = value;	}
		}

        public static TextWriter    Out
        {
            get { return _writer;   }
            set { _writer = value;  }
        }
        
        //Methods
		public static void		    Write(object value)
		{
            if(_writer != null)
				_writer.Write(value);
		}

		public static void		    WriteLine()
		{
			if(_writer != null)
				_writer.WriteLine();
		}

		public static void		    WriteLine(object value)
		{
            if(_writer != null)
                _writer.WriteLine(value);
		}

		public static string	    FormatMethod(ModelActionInfo info)
		{
			//Format:
			//	Command cmd = conn.createcommand();
			StringBuilder buffer = new StringBuilder(100);

			//Return
			if(info.RetVal != null)
			{
				Model output = info.RetVal as Model;
				if(output != null)
				{
					//Type
					if(info.Created)
					{
						buffer.Append(output.GetType().Name);
						buffer.Append(" ");
					}
							
					//Variable
					buffer.Append(output.Name);
					buffer.Append(output.Id);
					buffer.Append(" = ");
				}
			}
			
			//Call
			buffer.Append(info.Action.Model.Name);
			if(!info.Action.Method.IsStatic)
				buffer.Append(info.Action.Model.Id);
			buffer.Append(".");
			buffer.Append(info.Action.Name);
			
			//Parameters
			buffer.Append("(");
			if(info.Parameters != null)
			{
				int ordinal = 0;
				foreach(ModelParameter parameter in info.Parameters)
				{
					if(ordinal++ > 0)
						buffer.Append(", ");
					FormatValue(buffer, parameter.Type, parameter.Value.Value);
				}
			}
			buffer.Append(")");
			buffer.Append(";");
		
			//Output
			return buffer.ToString();
		}

		public static void		    FormatValue(StringBuilder builder, Type type, object value)
		{
			String prefix = null;
			String suffix = null;
			String cast   = null;
			String format = null;

			//Special types
			if(type == typeof(String))
			{
				//Strings
				prefix = "\""; 
				suffix = "\""; 
			}
			else if(type.IsEnum)
			{
				//Enums
				prefix = type.Name + ".";
				format = value.ToString().Replace(", ", " | " + prefix);
			}
			else if(type == typeof(bool))
			{
				//Bool (lowercase)
				format = value.ToString().ToLower();
			}
			else if(type == typeof(byte))
			{
				cast = "(byte)";
			}
			else if(type == typeof(short))
			{
				cast = "(short)";
			}
			else if(type.IsArray && value != null)
			{
				//new object[]{...}
				builder.Append("new ");
				//Note: Don't box integers, primarily since Int32 isn't a portable type (ie: Java)
				builder.Append(type == typeof(Int32[]) ? "int[]" : type.Name);
				builder.Append("{");

				//Recurse
				int ordinal = 0;
				Array items = (Array)value;
				foreach(object item in items)
				{
					if(ordinal++ > 0)
						builder.Append(", ");
					FormatValue(builder, type.GetElementType(), item);
				}

				builder.Append("}");
				return;	//Were done
			}

			//Default formating
			if(format == null && value != null)
				format = value.ToString();
			if(format == null)
				format = "null";

			//Prefix
			if(prefix != null)
				builder.Append(prefix);
			
			//Value
			if(cast != null)
				builder.Append(cast);
			builder.Append(format);

			//Suffix
			if(suffix != null)
				builder.Append(suffix);
		}
	}
}
