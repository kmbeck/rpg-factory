
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 *          --- DO NOT EDIT ---
 *  THIS CLASS CONTAINS GENERATED CODE!!!
 *          --- DO NOT EDIT ---
 * * * * */

public abstract class GScriptFlagLibrary : MonoBehaviour
{
    public static GScriptFlagLibrary inst;
	int TestFlag;
	bool TestFlag_001;
	float TestFlag_002;
	int TestFlag_003;
	int TestFlag_A;
	string TestStrFlag_001;
 
    void Start() {
        if (inst != null) {
            Destroy(this);
        }
        else {
            inst = this;
            Initialize();
        }
    }

    void Awake() {
        if (inst != null) {
            Destroy(this);
        }
        else {
            inst = this;
            Initialize();
        }
    }

    private void Initialize() {
		TestFlag = 987;
		TestFlag_001 = true;
		TestFlag_002 = 1564.486f;
		TestFlag_003 = 111111111;
		TestFlag_A = 200;
		TestStrFlag_001 = "Init Value";

    }
}

