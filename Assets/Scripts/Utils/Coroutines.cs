using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class Coroutines
{
    public static IEnumerator WaitThenExec(float sec, Action action) {
        yield return new WaitForSeconds(sec);
        action();
    }

    public static IEnumerator WaitForThenExec(Func<bool> predicate, Action action) {
        yield return new WaitUntil(predicate);
        action();
    }
}