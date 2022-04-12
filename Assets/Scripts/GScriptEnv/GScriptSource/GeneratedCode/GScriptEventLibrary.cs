
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 *          --- DO NOT EDIT ---
 *  THIS CLASS CONTAINS GENERATED CODE!!!
 *          --- DO NOT EDIT ---
 * * * * */

public static class GScriptEventLibrary
{
    public static void EVENT_EXECEVENTTEST001() {
EventInterface.EXEC_EVENT("ListTest001");
}

public static void EVENT_FLAGTEST001() {
GScriptFlagLibrary.TestFlag_003 = GScriptFlagLibrary.TestFlag_003 + 1;
EventInterface.DEBUG_PRINT("After: " + (GScriptFlagLibrary.TestFlag_003).ToString());
EventInterface.DEBUG_PRINT("After: " + (GScriptFlagLibrary.TestFlag_003).ToString());
}

public static void EVENT_LISTTEST001() {
List<string> words = new List<string>() {"this" , "is" , "a" , "test" , "!"};
List<int> nums = new List<int>() {5 , 10 , 15 , 20 , 25 , 30};
EventInterface.DEBUG_PRINT((nums[3] * 50).ToString());
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

