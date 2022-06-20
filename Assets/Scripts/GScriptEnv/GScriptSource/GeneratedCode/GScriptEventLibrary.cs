
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
GScriptFlagLibrary.TestFlag_003=GScriptFlagLibrary.TestFlag_003+1;
EventInterface.DEBUG_PRINT("After: "+(GScriptFlagLibrary.TestFlag_003).ToString());
}

public static void EVENT_LISTTEST001() {
List<string> words=new List<string>() {"hello","this","is","a","test","!"};
List<int> nums=new List<int>() {0,5,10,15,20,25,30};
nums.Add(5000);
words.Add("added");
EventInterface.DEBUG_PRINT(words[6]);
EventInterface.DEBUG_PRINT((nums[3]*50).ToString());
EventInterface.DEBUG_PRINT((1+2+3+4+5).ToString());
}

public static void EVENT_LISTTEST002() {
List<string> words=new List<string>() {"hello","this","is","a","test","!"};
List<int> nums=new List<int>() {0,5,10,15,20,25,30};
EventInterface.DEBUG_PRINT((nums[3]*50).ToString());
nums.Add(55);
EventInterface.DEBUG_PRINT((nums[6]).ToString());
}

public static void EVENT_MODULUSTEST_01() {
int a=7%3;
int b=6%4;
int c=55%5;
}

public static void EVENT_NEWEVENTINTERFACETEST() {
EventInterfaceTest.HAHAHA();
}


}

