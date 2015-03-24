﻿using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

namespace Shared
{
    public interface ITreeNode<T> : ITreeNode
    {
        ITreeNode<T> Root { get; }

        ITreeNode<T> Parent { get; set; }
        void SetParent(ITreeNode<T> Node, bool UpdateChildNodes = true);

        T Value { get; set; }

        TreeNodeList<T> Children { get; }
    }

    public interface ITreeNode : INotifyPropertyChanged
    {
        // all nodes along path toward root: Parent, Parent.Parent, Parent.Parent.Parent, ...
        IEnumerable<ITreeNode> Ancestors { get; }

        ITreeNode ParentNode { get; }

        // direct descendants
        IEnumerable<ITreeNode> ChildNodes { get; }

        // Children, Children[i].Children, ...
        IEnumerable<ITreeNode> Descendants { get; }

        //void OnNodeChanged(NodeChangeType changeType, ITreeNode node);
        //void OnParentChanged(NodeChangeType changeType, ITreeNode node);
        void OnAncestorChanged(NodeChangeType changeType, ITreeNode node);
        void OnDescendantChanged(NodeChangeType changeType, ITreeNode node);

        // distance from Root
        int Depth { get; }
        void OnDepthChanged();

        // distance from deepest descendant
        int Height { get; }
        void OnHeightChanged();
    }
}
