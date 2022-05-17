using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPA;

[DisallowMultipleComponent]
public class TestTree : MonoBehaviour {
    RPA_Tree tree01;
    // Start is called before the first frame update

    void Start() {
        tree01 = new RPA_Tree(transform.position);
        RPA_TreeNode node1 = tree01.SetBranch(Vector3.up, tree01.root);
        tree01.SetBranch(node1.Position + Random.onUnitSphere, node1);
    }

    // Update is called once per frame
    void Update() {
        if (tree01 != null) {
            tree01.ShowTreeStructure();
        }


    }

}
