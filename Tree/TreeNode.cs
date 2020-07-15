using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    /**/
    public class TreeNode<T>
    {
        public T m_Data;
        public TreeNode<T> m_Parent = null;
        public List<TreeNode<T>> m_Children = new List<TreeNode<T>>();


        
        public TreeNode( T data )
        {
            m_Data = data;
        }

        // TODO: add the typical tree node operation
    }
}
