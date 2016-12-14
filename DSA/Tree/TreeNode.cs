﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
    public class TreeNode<T> where T : IComparable
    {
        private T data;
        private NodeList<T> neighbors;

        public T Data
        {
            get { return data; }
            set { data = value; }
        }

        public NodeList<T> Neighbors
        {
            get { return neighbors; } 
            set{neighbors = value;}
        }
        public TreeNode(T data)
        {
            this.Data = data;
            this.Neighbors = null;
        }

        public TreeNode(T data, NodeList<T> neighbors)
        {
            this.Data = data;
            this.Neighbors = neighbors;
        }
    }
}
