
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
public static void EVENT_TESTEVENT1() {
int a = 10;
int b = 20;
float c = 55.123f;
int d = (a + b) * b;
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
