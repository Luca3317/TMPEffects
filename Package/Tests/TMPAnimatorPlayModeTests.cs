using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using TMPEffects.Components;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPEffects.TextProcessing;
using UnityEditor;
using System;

public class TMPAnimatorPlayModeTests
{
    const string testReferenceStr = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris in lectus eget nisl imperdiet gravida. Morbi mollis ipsum arcu, eleifend convallis erat rutrum eget. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Donec molestie vestibulum massa, quis pulvinar nunc facilisis vel. Aliquam consectetur metus metus, eu lacinia tortor vehicula id. Quisque placerat ac risus in egestas. Nam consequat id augue et fermentum. Curabitur consequat dui vitae odio porttitor volutpat. Integer elementum venenatis neque, sit amet tempus dolor iaculis a. Vivamus non imperdiet dui. Nullam iaculis auctor posuere. Suspendisse id dolor quis risus pharetra tincidunt. In enim nisi, auctor eu dictum sit amet, ultricies pellentesque ante. Sed scelerisque lorem id lectus sollicitudin, quis scelerisque justo ullamcorper.";
    string[] testStrs = new string[]
    {
        testStr0, testStr1, testStr2, testStr3, testStr4
    };
    const string testStr0 = "<wave>Lorem ipsum dolor sit amet</wave>, consectetur adipiscing elit. <jump>Mauris in lectus eget nisl imperdiet gravida.</jump> Morbi mollis ipsum arcu, eleifend convallis erat rutrum eget. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Donec molestie vestibulum massa, quis pulvinar nunc facilisis vel. Aliquam consectetur metus metus, eu lacinia tortor vehicula id. Quisque placerat ac risus in egestas. Nam consequat id augue et fermentum. Curabitur consequat dui vitae odio porttitor volutpat. Integer elementum venenatis neque, sit amet tempus dolor iaculis a. Vivamus non imperdiet dui. Nullam iaculis auctor posuere. Suspendisse id dolor quis risus pharetra tincidunt. In enim nisi, auctor eu dictum sit amet, ultricies pellentesque ante. Sed scelerisque lorem id lectus sollicitudin, quis scelerisque justo ullamcorper.";
    const string testStr1 = "<wave>Lorem ipsum <!wait=2>dolor sit amet</wave>, consectetur <?myEvent=myValue secondKey=secondValue>adipiscing elit. <jump>Mauris<!delay=4> in lectus eget nisl imperdiet gravida.</jump> Morbi mollis ipsum arcu, eleifend convallis erat rutrum eget. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Donec molestie vestibulum massa, quis pulvinar nunc facilisis vel. Aliquam consectetur metus metus, eu lacinia tortor vehicula id. Quisque placerat ac risus in egestas. Nam consequat id augue et fermentum. Curabitur consequat dui vitae odio porttitor volutpat. Integer elementum venenatis neque, sit amet tempus dolor iaculis a. Vivamus non imperdiet dui. Nullam iaculis auctor posuere. Suspendisse id dolor quis risus pharetra tincidunt. In enim nisi, auctor eu dictum sit amet, ultricies pellentesque ante. Sed scelerisque lorem id lectus sollicitudin, quis scelerisque justo ullamcorper.";
    const string testStr2 = "<wave><wave>Lorem <pivot>ipsu<-shake>m <!wait=2>dolor sit amet</wave>, conse<+pivot>ctetur <?myEvent=myValue secondKey=secondValue>adipiscing elit. <jump>Mauris<!delay=4> in lectus eget nisl imperdiet gravida.</jump> Morbi mollis ipsum arcu, eleifend convallis<+move> erat rutrum eget. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Donec molestie vestibulum massa, quis pulvinar nunc facilisis vel. Aliquam consectetur metus metus, eu lacinia tortor vehicula id. Quisque placerat ac risus in egestas. Nam consequat id augue et fermentum. Curabitur consequat dui vitae odio porttitor volutpat. Integer elementum venenatis neque, sit amet tempus dolor iaculis a. Vivamus non imperdiet dui. Nullam iaculis auctor posuere. Suspendisse id dolor quis risus pharetra tincidunt. In enim nisi, auctor eu dictum sit amet, ultricies pellentesque ante. Sed scelerisque lorem id lectus sollicitudin, quis scelerisque justo ullamcorper.";
    const string testStr3 = "<wave>Lorem ipsum <!wait=2>dolor sit amet</wave>, con<pivot><shake>sectetur <?myEvent=myValue secondKey=secondValue>adipiscing elit. <jump>Mauris<!delay=4> in lectus eget nisl imperdiet gravida.</jump> Morbi mollis ipsum arcu, eleifend convallis erat rutrum eget. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Donec molestie vestibulum massa, quis pulvinar nunc facilisis vel. Aliquam consectetur metus metus, eu lacinia tortor vehicula id. Quisque placerat ac risus in egestas. Nam consequat id augue et fermentum. <+move>Curabitur<!show> consequat dui <-char>vitae odio porttitor volutpat. <+char>Integer elementum venenatis neque,</!all> sit amet tempus dolor <-pivot>iaculis a.</all> Vivamus</+all></-all> non imperdiet dui. Nullam iaculis auctor posuere. </all>Suspendisse id dolor quis risus pharetra tincidunt. In enim nisi, auctor eu dictum sit amet, ultricies pellentesque ante. Sed scelerisque lorem id lectus sollicitudin, quis scelerisque justo ullamcorper.";
    const string testStr4 = "<wave>Lorem ipsum <!wait=2>dolor sit amet</>, consectetur <?myEvent=myValue secondKey=secondValue>adipiscing elit. <jump amp=10>Mauris<!delay=4> in lectus eget nisl imperdiet gravida.</> Mo<!show>r<wave>b</wave><wave></>i</!> mollis ipsum arcu, eleifend convallis erat rutrum eget. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Donec molestie vestibulum massa, quis pulvinar nunc facilisis vel. Aliquam consectetur metus metus, eu lacinia tortor vehicula id. Quisque placerat ac risus in egestas. Nam consequat id augue et fermentum. Curabitur consequat dui vitae odio porttitor volutpat. Integer elementum venenatis neque, sit amet tempus dolor iaculis a. Vivamus non imperdiet dui. Nullam iaculis auctor posuere. Suspendisse id dolor quis risus pharetra tincidunt. In enim nisi, auctor eu dictum sit amet, ultricies pellentesque ante. Sed scelerisque lorem id lectus sollicitudin, quis scelerisque justo ullamcorper.";

    TMPAnimator animator;
    TMPWriter writer;
    TMP_Text text;

    TagProcessor animationTagProcessor, showAnimationTagProcessor, hideAnimationTagProcessor, commandTagProcessor, eventTagProcessor;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        if (Application.isPlaying) yield break;

        TMP_PackageResourceImporter.ImportResources(true, false, false);
        yield return new EnterPlayMode();
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        yield return new ExitPlayMode();
    }

    [OneTimeSetUp]
    public void SetUp()
    {
        string[] folders = new string[] { "Assets", "Packages/com.luca3317.tmpeffects/Tests" };
        var guid = AssetDatabase.FindAssets($"TMPEffects-TestScene t:scene", folders)[0];
        var path = AssetDatabase.GUIDToAssetPath(guid);

        Scene scene = EditorSceneManager.LoadSceneInPlayMode(path, new LoadSceneParameters(LoadSceneMode.Single));
        EditorSceneManager.sceneLoaded += SetUp_Components;
    }

    [UnityTest]
    public IEnumerator CreationStateTests()
    {
        Assert.IsNotNull(animator);
        Assert.IsTrue(animator.enabled);
        Assert.IsTrue(animator.Tags.Count == 0);
        Assert.IsTrue(animator.AnimateOnStart);
        Assert.IsFalse(animator.AnimationsOverride);
        Assert.IsNull(animator.Database);
        Assert.IsTrue(animator.IsAnimating);
        Assert.IsTrue(animator.TextComponent == text);
        Assert.IsTrue(animator.UpdateFrom == TMPEffects.Components.Animator.UpdateFrom.Update);
        yield break;
    }

    [UnityTest]
    public IEnumerator AnimatingTests()
    {
        Assert.DoesNotThrow(() => animator.StartAnimating());
        Assert.DoesNotThrow(() => animator.StartAnimating());
        Assert.DoesNotThrow(() => animator.StopAnimating());
        Assert.DoesNotThrow(() => animator.StopAnimating());
        Assert.Throws<InvalidOperationException>(() => animator.UpdateAnimations(0f));

        animator.SetUpdateFrom(TMPEffects.Components.Animator.UpdateFrom.Script);
        Assert.DoesNotThrow(() => animator.UpdateAnimations(0f));

        Assert.Throws<InvalidOperationException>(() => animator.StartAnimating());
        Assert.Throws<InvalidOperationException>(() => animator.StopAnimating());
        

        yield break;
    }

    private void SetUp_Components(Scene scene, LoadSceneMode loadSceneMode)
    {
        EditorSceneManager.sceneLoaded -= SetUp_Components;

        GameObject go = new GameObject();
        text = go.AddComponent<TextMeshPro>();
        animator = go.AddComponent<TMPAnimator>();
        writer = go.AddComponent<TMPWriter>();

        //animator = GameObject.FindObjectOfType<TMPAnimator>();
        //writer = GameObject.FindObjectOfType<TMPWriter>();
        //text = GameObject.FindObjectOfType<TMP_Text>();

        if (animator == null) Debug.LogWarning("ANIMATOR NULL");
        if (writer == null) Debug.LogWarning("WRITER NULL");
        if (text == null) Debug.LogWarning("TEXT NULL");

        //animationTagProcessor = new TagProcessor(new TMPAnimationCategory(TMPAnimator.ANIMATION_PREFIX, animator.Database.BasicAnimationDatabase));
        //showAnimationTagProcessor = new TagProcessor(new TMPAnimationCategory(TMPAnimator.SHOW_ANIMATION_PREFIX, animator.Database.ShowAnimationDatabase));
        //hideAnimationTagProcessor = new TagProcessor(new TMPAnimationCategory(TMPAnimator.HIDE_ANIMATION_PREFIX, animator.Database.HideAnimationDatabase));
        //commandTagProcessor = new TagProcessor(new TMPCommandCategory(TMPWriter.COMMAND_PREFIX, writer.Database));
        //eventTagProcessor = new TagProcessor(new TMPEventCategory(TMPWriter.EVENT_PREFIX));
    }
}
