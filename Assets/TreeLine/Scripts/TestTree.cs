using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPA;

[DisallowMultipleComponent]
public class TestTree : MonoBehaviour {
    RPA_Tree tree01;
    // Start is called before the first frame update

    public Material material;
    TreeLine treeLine;
    void Start() {
        tree01 = new RPA_Tree(transform.position);
        /*
        node1 = tree01.GrowBranchNode(tree01.root, 5.0f);
        RPA_TreeNode node2 = tree01.GrowBranchNode(node1);
        RPA_TreeNode node3 = tree01.GrowBranchNode(node2);
        RPA_TreeNode node4 = tree01.GrowBranchNode(node3);
        RPA_TreeNode node5 = tree01.GrowBranchNode(node4);
        RPA_TreeNode node6 = tree01.GrowBranchNode(node5);
        RPA_TreeNode node7 = tree01.GrowBranchNode(node6);
        */
        treeLine = new TreeLine(material);
        
    }

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            tree01.Grow(10.0f, 15);
        }

        //tree01.ShowTreeLine();
        tree01.ShowTreeLine2();

    }

    public Transform testObj;
    Vector3 target;
    void TestBranchGrow() {
        testObj.position = Vector3.MoveTowards(testObj.position, target, 1.0f * Time.deltaTime);
        Debug.DrawLine(Vector3.zero, testObj.position);
    }

    void TestBranchGrow2() {


    }


    void fun1() {
        for (int i = 0; i < 10; i++) {

        }

    }

    void fun2() {

    }

    void OnDrawGizmos() {
        if (tree01 != null) {
            //tree01.ShowTreeNode();
        }

    }





}
