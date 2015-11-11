using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData.Core.UriParser.SemanticAst
{
    public class GroupByPropertyNode
    {

        public GroupByPropertyNode(string name, SingleValuePropertyAccessNode accessor)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            // OK for accessor to be null
            this.Name = name;
            this.Accessor = accessor;
        }

        public string Name { get; private set; }

        public SingleValuePropertyAccessNode Accessor { get; private set; }
       
        public IList<GroupByPropertyNode> Children { get; } = new List<GroupByPropertyNode>();
    }
}
