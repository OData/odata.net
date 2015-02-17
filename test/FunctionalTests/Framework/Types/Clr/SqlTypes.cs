//---------------------------------------------------------------------
// <copyright file="SqlTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace System.Data.Test.Astoria
{
    public static class SqlTypes
    {
        private static SqlDateTime dateTime = null;
        public static SqlDateTime DateTime
        {
            get
            {
                if (dateTime == null)
                    dateTime = new SqlDateTime();
                return dateTime;
            }
        }

        private static SqlSmallDateTime smallDateTime = null;
        public static SqlSmallDateTime SmallDateTime
        {
            get
            {
                if (smallDateTime == null)
                    smallDateTime = new SqlSmallDateTime();
                return smallDateTime;
            }
        }

        private static SqlFloat sqlFloat = null;
        public static SqlFloat Float
        {
            get
            {
                if (sqlFloat == null)
                    sqlFloat = new SqlFloat();
                return sqlFloat;
            }
        }

        private static SqlReal sqlReal = null;
        public static SqlReal Real
        {
            get
            {
                if (sqlReal == null)
                    sqlReal = new SqlReal();
                return sqlReal;
            }
        }
    }


    public class SqlDateTime : ClrDateTime
    {
        protected override DateTime MinValue
        {
            get
            {
                return new DateTime(1753, 1, 1);
            }
        }

        protected override DateTime MaxValue
        {
            get
            {
                return new DateTime(9999, 12, 31);
            }
        }

        protected override long TicksOfPrecision
        {
            get
            {
                return TimeSpan.TicksPerMillisecond * 10; //not really accurate, but the 3.3333 milliseconds precision is hard to deal with
            }
        }
    }

    public class SqlSmallDateTime : ClrDateTime
    {
        protected override DateTime MinValue
        {
            get
            {
                return new DateTime(1900, 1, 1);
            }
        }

        protected override DateTime MaxValue
        {
            get
            {
                return new DateTime(2079, 6, 6);
            }
        }

        protected override long TicksOfPrecision
        {
            get
            {
                // only accurate to the minute (ie, seconds always 0)
                return TimeSpan.TicksPerMinute;
            }
        }
    }

    public class SqlFloat : ClrDouble
    {
        protected override double[] SpecialValues
        {
            get
            {
                return new double[] { };
            }
        }
    }

    public class SqlReal : ClrFloat
    {
        // TODO: minimized range?
        protected override float[] SpecialValues
        {
            get
            {
                return new float[] { };
            }
        }
    }
}
