using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructureLearning.BinaryTree
{
    public static class Tree
    {
        /// <summary>
        /// 根据实体列表构建二叉树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static TreeNode<T> GenerateTree<T>(List<T> list, Func<T, bool> rootCondition, Func<T, T, bool> parentCondition)
            where T : Data
        {
            if (!list.Any())
            {
                return null;
            }

            var rootData = list.FirstOrDefault(x => rootCondition(x));
            TreeNode<T> root = new TreeNode<T>(rootData);
            Stack<TreeNode<T>> stackParentNodes = new Stack<TreeNode<T>>();
            stackParentNodes.Push(root);
            foreach (var item in list)
            {
                if (item == rootData)
                {
                    continue;
                }

                TreeNode<T> parent = stackParentNodes.Peek();
                while (!parentCondition(item, parent.Data))
                {
                    stackParentNodes.Pop();

                    if (stackParentNodes.Count == 0)
                    {
                        stackParentNodes.Push(root);
                        parent = root;
                        break;
                    }

                    parent = stackParentNodes.Peek();
                }

                var currentNode = new TreeNode<T>(item);
                if (parent.Left == null)
                {
                    parent.Left = currentNode;
                    currentNode.Parent = parent;
                }
                else
                {
                    if (parent.Left.Right == null)
                    {
                        parent.Left.Right = currentNode;
                        currentNode.Parent = parent.Left;
                    }
                    else
                    {
                        parent.Left.Right.Parent = currentNode;
                        currentNode.Right = parent.Left.Right;
                        currentNode.Parent = parent.Left;
                        parent.Left.Right = currentNode;
                    }
                }

                parent.Children.Add(item);

                stackParentNodes.Push(currentNode);
            }

            return root;
        }

        /// <summary>
        /// 后续遍历二叉树进行计算
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="leaveCompute"></param>
        /// <param name="branchCompute"></param>
        public static void Compute<T>(TreeNode<T> root, Action<T> leafCompute, Action<T, List<T>> branchCompute)
             where T : Data
        {
            if (root == null)
            {
                return;
            }

            TreeNode<T> currentNode = null,
                preNode = null;
            Stack<TreeNode<T>> stackParentNodes = new Stack<TreeNode<T>>();
            stackParentNodes.Push(root);
            while (stackParentNodes.Any())
            {
                currentNode = stackParentNodes.Peek();
                if ((currentNode.Left == null && currentNode.Right == null)
                    || (preNode != null) && (preNode == currentNode.Left || preNode == currentNode.Right))
                {
                    preNode = currentNode;
                    if (currentNode.Children.Any())
                    {
                        currentNode.Data.LeavesCount = currentNode.Children.Sum(x => x.LeavesCount);
                        branchCompute?.Invoke(currentNode.Data, currentNode.Children);
                    }
                    else
                    {
                        currentNode.Data.LeavesCount = 1;
                        leafCompute?.Invoke(currentNode.Data);
                    }
                    stackParentNodes.Pop();
                }
                else
                {
                    if (currentNode.Right != null)
                    {
                        stackParentNodes.Push(currentNode.Right);
                    }
                    if (currentNode.Left != null)
                    {
                        stackParentNodes.Push(currentNode.Left);
                    }
                }
            }
        }

        /// <summary>
        /// 计算指定节点极其父级
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="branchCompute"></param>
        public static void ComputeParent<T>(TreeNode<T> node, Action<T> leafCompute, Action<T, List<T>> branchCompute)
            where T : Data
        {
            if (node == null)
            {
                return;
            }

            TreeNode<T> currentNode = node;

            if (node.Children.Any())
            {
                currentNode.Data.LeavesCount = currentNode.Children.Sum(x => x.LeavesCount);
                branchCompute?.Invoke(currentNode.Data, currentNode.Children);
            }
            else
            {
                currentNode.Data.LeavesCount = 1;
                currentNode.Data.IsLeaf = true;
                leafCompute?.Invoke(currentNode.Data);
            }

            while (currentNode != null)
            {
                var parentNode = currentNode.Parent;
                if (parentNode != null && parentNode.Left == currentNode)
                {
                    branchCompute?.Invoke(parentNode.Data, parentNode.Children);
                }

                currentNode = parentNode;
            }
        }

        /// <summary>
        /// 查找符合指定条件的节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TreeNode<T> FindNode<T>(TreeNode<T> node, Func<T, bool> condition)
             where T : Data
        {
            if (node == null)
            {
                return null;
            }

            Stack<TreeNode<T>> stackParentsNodes = new Stack<TreeNode<T>>();
            TreeNode<T> currentNode = node;
            while (currentNode != null || stackParentsNodes.Any())
            {
                if (currentNode != null)
                {
                    if (condition(currentNode.Data))
                    {
                        return currentNode;
                    }

                    stackParentsNodes.Push(currentNode);
                    currentNode = currentNode.Left;
                }
                else
                {
                    currentNode = stackParentsNodes.Pop().Right;
                }
            }

            return null;
        }

        /// <summary>
        /// 查找指定ID节点，右子树优先，便于优先查找出上层节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">开始查找的节点</param>
        /// <param name="condition">匹配条件</param>
        /// <returns></returns>
        public static TreeNode<T> FindNodeRightFirst<T>(TreeNode<T> node, Func<T, bool> condition)
             where T : Data
        {
            if (node == null)
            {
                return null;
            }

            Stack<TreeNode<T>> stackParentsNodes = new Stack<TreeNode<T>>();
            TreeNode<T> currentNode = node;
            while (currentNode != null || stackParentsNodes.Any())
            {
                if (currentNode != null)
                {
                    if (condition(currentNode.Data))
                    {
                        return currentNode;
                    }

                    stackParentsNodes.Push(currentNode);
                    currentNode = currentNode.Right;
                }
                else
                {
                    currentNode = stackParentsNodes.Pop().Left;
                }
            }

            return null;
        }

        /// <summary>
        /// 统计叶子节点数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        public static TreeNode<T> CountLeaves<T>(List<T> list, Func<T, bool> rootCondition, Func<T, T, bool> parentCondition)
             where T : Data
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }

            TreeNode<T> root = GenerateTree(list, rootCondition, parentCondition);
            if (root == null)
            {
                return null;
            }

            Compute(root, null, null);
            if (!root.Children.Any())
            {
                root.Data.LeavesCount = 0;
            }

            return root;
        }

        /// <summary>
        /// 后续遍历二叉树，对节点进行nodeAction操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="nodeAction"></param>
        public static void Traversal<T>(TreeNode<T> root, Action<TreeNode<T>> nodeAction)
            where T : Data
        {
            if (root == null)
            {
                return;
            }

            TreeNode<T> currentNode = null,
                preNode = null;
            Stack<TreeNode<T>> stackParentNodes = new Stack<TreeNode<T>>();
            stackParentNodes.Push(root);

            while (stackParentNodes.Any())
            {
                currentNode = stackParentNodes.Peek();
                if ((currentNode.Left == null && currentNode.Right == null)
                    || (preNode != null && (preNode == currentNode.Left || preNode == currentNode.Right)))
                {
                    nodeAction(currentNode);
                    preNode = currentNode;
                    stackParentNodes.Pop();
                }
                else
                {
                    if (currentNode.Right != null)
                    {
                        stackParentNodes.Push(currentNode.Right);
                    }
                    if (currentNode.Left != null)
                    {
                        stackParentNodes.Push(currentNode.Left);
                    }
                }
            }
        }
    }
}
