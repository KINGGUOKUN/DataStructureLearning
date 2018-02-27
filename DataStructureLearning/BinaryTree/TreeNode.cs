using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructureLearning.BinaryTree
{
    /// <summary>
    /// 二叉树节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T>
        where T : Data
    {
        public TreeNode()
        {
            this.Children = new List<T>();
        }

        public TreeNode(T data)
            : this()
        {
            this.Data = data;
        }

        /// <summary>
        /// 节点对应数据节点
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 树节点
        /// </summary>
        public TreeNode<T> Parent { get; set; }

        /// <summary>
        /// 左子树
        /// </summary>
        public TreeNode<T> Left { get; set; }

        /// <summary>
        /// 右子树
        /// </summary>
        public TreeNode<T> Right { get; set; }

        /// <summary>
        /// 该节点对应业务节点的子业务节点集合
        /// </summary>
        public List<T> Children { get; private set; }
    }
}
