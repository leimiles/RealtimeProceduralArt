using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPA {
    public class TreeLine {
        Mesh mesh;
        public Mesh TreeLineMesh {
            get {
                return mesh;
            }
        }
        Material material;
        public Material TreeLineMaterial {
            get {
                return material;
            }
        }

        List<Matrix4x4> localToMatrices;

        public TreeLine(Material material) {
            Vector3[] vertices = new Vector3[3];
            vertices[0] = Vector3.right * 0.05f;
            vertices[1] = Vector3.left * 0.05f;
            vertices[2] = Vector3.forward;


            Color[] colors = new Color[3];
            colors[0] = colors[1] = colors[2] = Color.yellow;

            int[] triangles = new int[3] { 0, 1, 2 };

            mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.colors = colors;
            mesh.triangles = triangles;
            this.material = material;
            localToMatrices = new List<Matrix4x4>();
        }


        public void Draw() {
            if (localToMatrices != null && localToMatrices.Count > 1 && localToMatrices.Count < 1023) {
                Graphics.DrawMeshInstanced(mesh, 0, material, localToMatrices.ToArray());
            }


        }

        public void Clear() {
            if (localToMatrices != null) {
                localToMatrices.Clear();
            }
        }

    }

    public class RPA_Tree {
        //List<List<RPA_TreeNode>> treeGrades;
        Dictionary<int, List<RPA_TreeNode>> tree;
        public RPA_TreeRoot root;
        int gradeCount;

        TreeLine treeLine;
        public int GradeCount {
            get {
                return gradeCount;
            }
        }
        public RPA_Tree(Vector3 position) {
            root = new RPA_TreeRoot(position);
            tree = new Dictionary<int, List<RPA_TreeNode>>();
            tree[0] = new List<RPA_TreeNode>();
            tree[0].Add(root);
            branchNodeMatrices = new List<Matrix4x4>();
        }

        public void SetTreeLine(TreeLine treeLine) {
            this.treeLine = treeLine;
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

        int grade = 0;
        public void Grow(float length, int branchNumberMax) {
            grade++;
            if (grade == 1) {
                RPA_TreeBranch newBranch = GrowBranchNode(root, length) as RPA_TreeBranch;
                branchNodeMatrices.Add(newBranch.LocalToWorld);
            } else {
                foreach (RPA_TreeNode parentNode in tree[grade - 1]) {
                    int number = Random.Range(0, branchNumberMax);
                    for (int i = 0; i < number; i++) {
                        RPA_TreeBranch newBranch = GrowBranchNode(parentNode) as RPA_TreeBranch;
                        branchNodeMatrices.Add(newBranch.LocalToWorld);
                    }
                }
            }
        }

        public RPA_TreeNode GrowBranchNode(RPA_TreeNode parentNode, float length = 1.0f) {
            float x;
            float y;
            float z;
            float branchLength;
            if (parentNode.GradeIndex == 0) {
                branchLength = length;
                x = Random.Range(-0.1f, 0.1f);
                y = Random.Range(0.8f, 1.0f);
                z = Random.Range(-0.1f, 0.1f);
            } else {
                x = Random.Range(-1.0f, 1.0f);
                y = Random.Range(0.15f, 1.0f);
                z = Random.Range(-1.0f, 1.0f);
                branchLength = Random.Range(0.01f, (parentNode as RPA_TreeBranch).Length);
            }

            Vector3 branchDirection = new Vector3(x, y, z);
            branchDirection = Vector3.Normalize(branchDirection);
            Vector3 position = branchDirection *= branchLength;

            return SetBranch(parentNode.Position + position, parentNode);
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



        // 
        public void ShowTreeLine() {
            for (int i = 0; i < gradeCount; i++) {
                if (i == 0) {
                    continue;
                }
                foreach (RPA_TreeNode treeNode in tree[i]) {
                    RPA_TreeBranch branch = treeNode as RPA_TreeBranch;
                    Debug.DrawLine(branch.Position, branch.parent.Position);
                }
            }
        }

        List<Matrix4x4> branchNodeMatrices;
        public void ShowTreeLine2() {
            if (branchNodeMatrices != null && branchNodeMatrices.Count < 1023) {
                Graphics.DrawMeshInstanced(treeLine.TreeLineMesh, 0, treeLine.TreeLineMaterial, branchNodeMatrices.ToArray());
            }

        }


        // must be called in OnDrawGizmos
        public void ShowTreeNode() {
            for (int i = 0; i < gradeCount; i++) {
                foreach (RPA_TreeNode treeNode in tree[i]) {
                    Gizmos.DrawSphere(treeNode.Position, 0.1f);
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
            set {
                position = value;
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
        private Matrix4x4 localToWorld;
        public Matrix4x4 LocalToWorld {
            get {
                return localToWorld;
            }
        }
        public RPA_TreeNode parent;
        float distanceToParent;
        Vector3 directionToParent;
        public float Length {
            get {
                return distanceToParent;
            }
        }
        public RPA_TreeBranch(Vector3 position, RPA_TreeNode parent) {
            if (parent == null) {
                Debug.Log("parent must be correct.");
                return;
            }
            this.position = position;
            this.parent = parent;
            this.gradeIndex = parent.GradeIndex + 1;
            distanceToParent = Vector3.Distance(position, parent.Position);
            directionToParent = (parent.Position - position).normalized;
            SettNodeTranform();

        }

        void SettNodeTranform() {
            Vector3 scale = Vector3.one;
            scale.z *= distanceToParent;
            Quaternion quaternion = Quaternion.LookRotation(directionToParent, Vector3.up);
            localToWorld = Matrix4x4.TRS(this.position, quaternion, scale);

        }






    }
}
