//---------------------------------------------------------------------
// <copyright file="AlternativeContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Client;

namespace AstoriaUnitTests.DataWebClientCSharp.AlternativeNS
{
    public class BaseballLeague
    {
        private List<BaseballTeam> teams = new List<BaseballTeam>();

        public int ID { get; set; }
        public string Name { get; set; }

        public List<BaseballTeam> Teams
        {
            get
            {
                //List<BaseballTeam> teams = (from l in ReadOnlyTestContext.CreateBaseLineContext().Leagues
                //                           where l.ID == this.ID
                //                           from t in l.Teams
                //                           select new BaseballTeam()
                //                           {
                //                               TeamID = t.TeamID,
                //                               City = t.City,
                //                               HomeStadium = null,
                //                               Mascot = null,
                //                               Players = null,
                //                               TeamName = t.TeamName
                //                           }).ToList();

                //return (List<BaseballTeam>)AlternativeContext.CheckToSeeIfIncluded(this, "Teams", teams);
                return null;
            }

            set
            {
                teams = value;
            }
        }

        public DateTimeOffset ConceptionDate { get; set; }

        public override int GetHashCode()
        {
            return this.ID;
        }

        public override bool Equals(object obj)
        {
            BaseballLeague left = this;
            BaseballLeague right = obj as BaseballLeague;

            if ((right == null) ||
               (right.ID != left.ID) ||
               (right.Name != left.Name) ||
               (right.ConceptionDate != left.ConceptionDate))
            {
                return false;
            }

            if (left.Teams != null && right.Teams != null)
            {

                IEnumerator<BaseballTeam> leftTeams = left.Teams.GetEnumerator();
                IEnumerator<BaseballTeam> rightTeams = right.Teams.GetEnumerator();

                //if (left.Teams.Count != right.Teams.Count)
                //    return false;

                //while (leftTeams.MoveNext() && rightTeams.MoveNext())
                //{
                //    if (!leftTeams.Current.Equals(rightTeams.Current))
                //    {
                //        return false;
                //    }
                //}

            }

            return true;
        }
    }

    [Microsoft.OData.Client.Key("TeamID")]
    public class BaseballTeam
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public string City { get; set; }

        private Stadium stadium;
        private Mascot mascot;
        private List<Player> players;

        public byte[] Photo
        {
            get { return new byte[] { 9, 9, 9, 9, 9, 9, 9, 9 }; }
            set { value = null; }
        }

        public Stadium HomeStadium
        {
            get
            {
                return this.stadium;
            }
            set
            {
                stadium = value;
            }
        }

        public Mascot Mascot
        {
            get
            {
                return this.mascot;
            }
            set
            {
                mascot = value;
            }
        }

        public List<Player> Players
        {
            get
            {
                return this.Players;
            }
            set
            {
                players = value;
            }
        }
    }

    public class AllStarBaseballTeam : BaseballTeam
    {
    }
}
