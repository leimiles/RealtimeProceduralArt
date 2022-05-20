using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPA;

[DisallowMultipleComponent]
public class TestTree : MonoBehaviour {
    RPA_Tree tree01;
    // Start is called before the first frame update
    TreeLine treeLine;
    public Material material;
    void Start() {
        tree01 = new RPA_Tree(transform.position);
        treeLine = new TreeLine(material);
        tree01.SetTreeLine(treeLine);
    }

    void Update() {
        if (Input.GetMouseButtonUp(0)) {
            tree01.Grow(10.0f, 8);
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


    void OnDrawGizmos() {
        if (tree01 != null) {
            tree01.ShowTreeNode();
        }

    }





}
