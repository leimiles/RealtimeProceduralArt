using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPA {
    public class TreeLine {

    }

    public class RPA_Tree {
        //List<List<RPA_TreeNode>> treeGrades;
        Dictionary<int, List<RPA_TreeNode>> tree;
        public RPA_TreeRoot root;
        int gradeCount;
        public RPA_Tree(Vector3 position) {
            root = new RPA_TreeRoot(position);
            tree = new Dictionary<int, List<RPA_TreeNode>>();
            tree[0] = new List<RPA_TreeNode>();
            tree[0].Add(root);
        }

        public RPA_TreeNode SetBranch(Vector3 position, RPA_TreeNode parent) {
            if (!tree.ContainsKey(parent.GradeIndex)) {
                Debug.Log("branch parent is not correct");
                return null;
            } else {
                int gradeNumber = parent.GradeIndex + 1;
                if (!tree.ContainsKey(gradeNumber)) {
                    tree[gradeNumber] = new List<RPA_TreeNode>();
                }
                RPA_TreeBranch branch = new RPA_TreeBranch(position, parent);
                tree[gradeNumber].Add(branch);
                gradeCount = gradeNumber + 1;
                return branch;
            }
        }

        public void printTreeStructure() {
            for (int i = 0; i < gradeCount; i++) {
                string info = "Grade[" + i + "]: ";
                foreach (RPA_TreeNode node in tree[i]) {
                    info += node.Position.ToString() + " | ";
                }
                Debug.Log(info);
            }
        }

        public void ShowTreeStructure() {
            for (int i = 0; i < gradeCount; i++) {
                if (i == 0) {
                    continue;
                }
                foreach (RPA_TreeNode treeNode in tree[i]) {
                    RPA_TreeBranch branch = treeNode as RPA_TreeBranch;
                    Debug.DrawLine(branch.parent.Position, branch.Position);

                }
            }
        }
    }

    public class RPA_TreeNode {
        protected Vector3 position;
        public Vector3 Position {
            get {
                return position;
            }
        }
        protected int gradeIndex;
        public int GradeIndex {
            get {
                return gradeIndex;
            }
        }
    }

    public class RPA_TreeRoot : RPA_TreeNode {
        public RPA_TreeRoot(Vector3 position) {
            this.position = position;
            this.gradeIndex = 0;
        }
    }

    public class RPA_TreeBranch : RPA_TreeNode {
        public RPA_TreeNode parent;
        public RPA_TreeBranch(Vector3 position, RPA_TreeNode parent) {
            if (parent == null) {
                Debug.Log("parent must be correct.");
                return;
            }
            this.position = position;
            this.parent = parent;
            this.gradeIndex = parent.GradeIndex + 1;
        }


    }
}
