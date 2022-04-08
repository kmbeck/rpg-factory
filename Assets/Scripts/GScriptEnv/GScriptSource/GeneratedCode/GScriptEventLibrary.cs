
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 *          --- DO NOT EDIT ---
 *  THIS CLASS CONTAINS GENERATED CODE!!!
 *          --- DO NOT EDIT ---
 * * * * */

public abstract class GScriptEventLibrary : MonoBehaviour
{
    public static GScriptEventLibrary inst;

    void Start() {
        if (inst != null) {
            Destroy(this);
        }
        else {
            inst = this;
        }
    }

    void Awake() {
        if (inst != null) {
            Destroy(this);
        }
        else {
            inst = this;
        }
    }
public static void EVENT_FLAGTEST001() {
EventInterface.DEBUG_PRINT(GScriptFlagLibrary.inst.TestStrFlag_001);
GScriptFlagLibrary.inst.TestStrFlag_001 = "new val";
EventInterface.DEBUG_PRINT(GScriptFlagLibrary.inst.TestStrFlag_001);
}

public static void EVENT_MODULUSTEST_01() {
int a = 7 % 3;
int b = 6 % 4;
int c = 55 % 5;
}

public static void EVENT_TESTEVENT1() {
int a = 10;
int b = 20;
float c = 55.123f;
int d = a + b * b;
string test_msg = "This is a test message!";
if (b > a) {
	EventInterface.DEBUG_PRINT("b > a");
}
else if (a > b) {
	EventInterface.DEBUG_PRINT("a > b");
}
else {
	EventInterface.DEBUG_PRINT("a = b");
}
string s = "save me!";
}

public static void EVENT_TESTEVENT2() {
EventInterface.DEBUG_PRINT("Aaskdljfas;kf");
}

public static void EVENT_TESTEVENT22() {
int a = 10;
int b = 20;
}

public static void EVENT_TESTEVENT55() {
string a = "Test.";
string b = "10";
EventInterface.DEBUG_PRINT(a);
}

}
