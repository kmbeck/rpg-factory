using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    string TEST_STATEMENT_A = "PRINT_MSG_TO_CONSOLE(\"Test Message.\")";

    string TEST_STATEMENT_B = "int a = 10\nint b = 11\nint c = (a + b) * b";

    string TEST_STATEMENT_C = "if (a = b)\n\tSET_FLAG(\"Flag Name\", 1)";

    // Start is called before the first frame update
    void Start()
    {
        GScriptCompiler compiler = new GScriptCompiler();
        compiler.validate(TEST_STATEMENT_C);
        // foreach (Statement s in program) {
        //     Debug.Log(s.ToString());
        // }
    }

    public void DebugDisplayTokens(List<Token> tokens) {
        foreach (Token t in tokens) {
            Debug.Log($"Type: {t.type}, Value: {t.value}");
        }
    }
}