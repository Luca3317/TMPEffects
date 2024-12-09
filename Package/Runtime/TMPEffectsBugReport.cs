#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

internal static class TMPEffectsEditorPrefsKeys
{
    public const string OptOutKey =  "TMPEffects.BugReport.OptOutOfBugReport";
    public const string RanIntoMissingReflectedPowerSlider = "TMPEffects.RanInto.MissingReflectedPowerSlider";
}

internal static class TMPEffectsBugReport
{
    public static void BugReportPrompt(System.Exception exception)
    {
        BugReportPrompt(exception.ToString());
    }

    public static void BugReportPrompt(string message)
    {
#if UNITY_EDITOR
        // Have to do this check before since if opted out will return true
        if (!EditorUtility.GetDialogOptOutDecision(DialogOptOutDecisionType.ForThisMachine, TMPEffectsEditorPrefsKeys.OptOutKey))
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
                    EditorUtility.SetDialogOptOutDecision(DialogOptOutDecisionType.ForThisMachine, TMPEffectsEditorPrefsKeys.OptOutKey, true);
            }
        }
#endif
        Debug.LogWarning("It seems you ran into a bug: " + message +
                         "\nPlease take the time to make a bug report on GitHub:\nhttps://github.com/Luca3317/TMPEffects.BugReport");
    }
}