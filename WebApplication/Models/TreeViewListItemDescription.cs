using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApplication.Models.CodeFirst;

namespace WebApplication.Models
{
    public class TaskTreeNode
    {
        public TaskTreeNode(WebApplication.Models.CodeFirst.Task node)
        {
            Node = node;
        }

        public TaskTreeNode(WebApplication.Models.CodeFirst.Task node, WebApplication.Models.CodeFirst.Task parent)
        {
            Node = node;
            ParentNode = parent;
        }

        public WebApplication.Models.CodeFirst.Task Node { get; set; }

        public WebApplication.Models.CodeFirst.Task ParentNode { get; set; }
    }

    public class LabelTreeNode
    {
        public LabelTreeNode(Label node)
        {
            Node = node;
        }

        public LabelTreeNode(Label node, Label parent)
        {
            Node = node;
            ParentNode = parent;
        }

        public Label Node { get; set; }

        public Label ParentNode { get; set; }
    }
}
