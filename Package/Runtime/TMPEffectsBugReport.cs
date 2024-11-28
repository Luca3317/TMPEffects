using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal static class TMPEffectsBugReport
{
    public const string OptOutKey =  "TMPEffects.BugReport.MayOpenWindowHuh";
    
    public static void BugReportPrompt(System.Exception exception)
    {
        BugReportPrompt(exception.ToString());
    }

    public static void BugReportPrompt(string message)
    {
        message = string.Join(" ", Enumerable.Repeat(message, 100));

#if UNITY_EDITOR
        // Have to do this check before since if opted out will return true
        if (!EditorUtility.GetDialogOptOutDecision(DialogOptOutDecisionType.ForThisMachine, OptOutKey))
        {
            if (message.Length > 500)
            {
                message = message.Substring(0, 500) + "...";
            }


            int result = EditorUtility.DisplayDialogComplex("TMPEffects Bug Report",
                "It seems you ran into a bug:\n" + message +
                "\n\nPlease take the time to make a bug report on GitHub.\nMake sure to include the full console output, including the stack trace.",
                "Open GitHub", "Not now", "Don't show this message again");

            if (result == 0)
            {
                Application.OpenURL(
                    "https://github.com/Luca3317/TMPEffects/issues/new?assignees=&labels=bug&projects=&template=bug_report.md&title=");
            }
            else if (result == 2)
            {
                bool undo = EditorUtility.DisplayDialog("TMPEffects Bug Report Opt out",
                    "You have opted out of the bug report window.\nIf you ever want to undo this, you can do so in Project Settings/TMPEffects.",
                    "Undo now", "Ok");
                if (!undo)
                    EditorUtility.SetDialogOptOutDecision(DialogOptOutDecisionType.ForThisMachine, OptOutKey, true);
            }
        }
#endif
        Debug.LogWarning("It seems you ran into a bug: " + message +
                         "\nPlease take the time to make a bug report on GitHub:\nhttps://github.com/Luca3317/TMPEffects.BugReport");
    }
}