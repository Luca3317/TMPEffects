using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using TMPEffects.Components;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPEffects.TextProcessing;
using TMPEffects.EffectCategories;
using System.Linq;
using UnityEditor;

public class TextProcessorTests
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
    public IEnumerator PreprocessTest()
    {
        TMPTextProcessor processor = new TMPTextProcessor(text);

        processor.AddProcessor(TMPWriter.EVENT_PREFIX, eventTagProcessor);
        processor.AddProcessor(TMPWriter.COMMAND_PREFIX, commandTagProcessor);
        processor.AddProcessor(TMPAnimator.ANIMATION_PREFIX, animationTagProcessor);
        processor.AddProcessor(TMPAnimator.SHOW_ANIMATION_PREFIX, showAnimationTagProcessor);
        processor.AddProcessor(TMPAnimator.HIDE_ANIMATION_PREFIX, hideAnimationTagProcessor);

        foreach (var str in testStrs)
        {
            Assert.AreEqual(testReferenceStr + " ", processor.PreprocessText(str));
        }

        yield break;
    }

    [UnityTest]
    public IEnumerator ZEmptyPreprocessTest()
    {
        TMPTextProcessor processor = new TMPTextProcessor(text);

        processor.AddProcessor(TMPWriter.EVENT_PREFIX, eventTagProcessor);
        processor.AddProcessor(TMPWriter.COMMAND_PREFIX, commandTagProcessor);
        processor.AddProcessor(TMPAnimator.ANIMATION_PREFIX, animationTagProcessor);
        processor.AddProcessor(TMPAnimator.SHOW_ANIMATION_PREFIX, showAnimationTagProcessor);
        processor.AddProcessor(TMPAnimator.HIDE_ANIMATION_PREFIX, hideAnimationTagProcessor);

        Assert.AreEqual(" ", processor.PreprocessText(null));
        Assert.AreEqual(" ", processor.PreprocessText(""));
        Assert.AreEqual("  ", processor.PreprocessText(" "));

        yield break;
    }

    [UnityTest]
    public IEnumerator IndexAdjustmentTest_Closed()
    {
        text.SetText("<color=red><s><b></b><style=h3><wave></wave>");
        yield return null;
        var basictagsss = animator.BasicTags.TagsAt(0).ToList();
        Assert.AreEqual(1, basictagsss.Count);
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(0, basictagsss[0].Indices.StartIndex);
        Assert.AreEqual(0, basictagsss[0].Indices.EndIndex);
        Assert.AreEqual(0, basictagsss[0].Indices.OrderAtIndex);

        text.SetText("<color=red><s><b></b><style=h3><wave></wave>");
        yield return null;
        var basictagss = animator.BasicTags.TagsAt(0).ToList();
        Assert.AreEqual(1, basictagss.Count);
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(0, basictagss[0].Indices.StartIndex);
        Assert.AreEqual(0, basictagss[0].Indices.EndIndex);
        Assert.AreEqual(0, basictagss[0].Indices.OrderAtIndex);


        text.SetText("<wave>Lorem ipsum dolor sit amet.</>Posttext");
        yield return null;
        var basictags = animator.BasicTags.TagsAt(0).ToList();
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(1, basictags.Count);
        Assert.AreEqual(0, basictags[0].Indices.StartIndex);
        Assert.AreEqual(27, basictags[0].Indices.EndIndex);
        Assert.AreEqual(0, basictags[0].Indices.OrderAtIndex);

        text.SetText("<color=red><wave>Lorem<s> ipsum <b>dol</s>or sit a</b>met.</></color>Posttext");
        yield return null;
        basictags = animator.BasicTags.TagsAt(0).ToList();
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(1, basictags.Count);
        Assert.AreEqual(0, basictags[0].Indices.StartIndex);
        Assert.AreEqual(27, basictags[0].Indices.EndIndex);
        Assert.AreEqual(0, basictags[0].Indices.OrderAtIndex);

        text.SetText("<color=red>Lorem<wave>mm<s> ipsum <b>dol</s>or sit a</b>m</>et.</color>Posttext");
        yield return null;
        basictags = animator.BasicTags.TagsAt(5).ToList();
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(1, basictags.Count);
        Assert.AreEqual(5, basictags[0].Indices.StartIndex);
        Assert.AreEqual(26, basictags[0].Indices.EndIndex);
        Assert.AreEqual(0, basictags[0].Indices.OrderAtIndex);

        text.SetText("<wave>Lorem ip<!show>sum dolor sit amet.</>Pos</!show>ttext");
        yield return null;
        basictags = animator.BasicTags.TagsAt(0).ToList();
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(1, basictags.Count);
        Assert.AreEqual(0, basictags[0].Indices.StartIndex);
        Assert.AreEqual(27, basictags[0].Indices.EndIndex);
        Assert.AreEqual(0, basictags[0].Indices.OrderAtIndex);

        var commandtags = writer.CommandTags.TagsAt(8).ToList();
        Assert.AreEqual(1, writer.CommandTags.Count);
        Assert.AreEqual(1, commandtags.Count);
        Assert.AreEqual(8, commandtags[0].Indices.StartIndex);
        Assert.AreEqual(30, commandtags[0].Indices.EndIndex);
        Assert.AreEqual(0, commandtags[0].Indices.OrderAtIndex);

        text.SetText("<color=red>L<s>o<wave>r</color>em<b> ip<!show>sum d</b>olor sit amet.</>Pos</!show>tte</s>xt");
        yield return null;
        basictags = animator.BasicTags.TagsAt(2).ToList();
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(1, basictags.Count);
        Assert.AreEqual(2, basictags[0].Indices.StartIndex);
        Assert.AreEqual(27, basictags[0].Indices.EndIndex);
        Assert.AreEqual(0, basictags[0].Indices.OrderAtIndex);

        commandtags = writer.CommandTags.TagsAt(8).ToList();
        Assert.AreEqual(1, writer.CommandTags.Count);
        Assert.AreEqual(1, commandtags.Count);
        Assert.AreEqual(8, commandtags[0].Indices.StartIndex);
        Assert.AreEqual(30, commandtags[0].Indices.EndIndex);
        Assert.AreEqual(0, commandtags[0].Indices.OrderAtIndex);

        text.SetText("<color=red><style=h3>L<s>o<wave>r</color>em<b> ip<!show>s<style=h3>um d</b>olor sit amet.</>Pos</!show>tte</s>xt");
        yield return null;
        basictags = animator.BasicTags.TagsAt(2).ToList();
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(1, basictags.Count);
        Assert.AreEqual(2, basictags[0].Indices.StartIndex);
        Assert.AreEqual(27, basictags[0].Indices.EndIndex);
        Assert.AreEqual(0, basictags[0].Indices.OrderAtIndex);

        commandtags = writer.CommandTags.TagsAt(8).ToList();
        Assert.AreEqual(1, writer.CommandTags.Count);
        Assert.AreEqual(1, commandtags.Count);
        Assert.AreEqual(8, commandtags[0].Indices.StartIndex);
        Assert.AreEqual(30, commandtags[0].Indices.EndIndex);
        Assert.AreEqual(0, commandtags[0].Indices.OrderAtIndex);

        yield break;
    }

    [UnityTest]
    public IEnumerator IndexAdjustmentTest_UnClosed()
    {
        text.SetText("<wave>Lorem ipsum dolor sit amet.Posttext");
        yield return null;
        var basictags = animator.BasicTags.TagsAt(0).ToList();
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(1, basictags.Count);
        Assert.AreEqual(0, basictags[0].Indices.StartIndex);
        Assert.AreEqual(-1, basictags[0].Indices.EndIndex);
        Assert.AreEqual(0, basictags[0].Indices.OrderAtIndex);

        text.SetText("<color=red><wave>Lorem<s> ipsum <b>dol</s>or sit a</b>met.</color>Posttext");
        yield return null;
        basictags = animator.BasicTags.TagsAt(0).ToList();
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(1, basictags.Count);
        Assert.AreEqual(0, basictags[0].Indices.StartIndex);
        Assert.AreEqual(-1, basictags[0].Indices.EndIndex);
        Assert.AreEqual(0, basictags[0].Indices.OrderAtIndex);

        text.SetText("<color=red>Lorem<wave>mm<s> ipsum <b>dol</s>or sit a</b>met.</color>Posttext");
        yield return null;
        basictags = animator.BasicTags.TagsAt(5).ToList();
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(1, basictags.Count);
        Assert.AreEqual(5, basictags[0].Indices.StartIndex);
        Assert.AreEqual(-1, basictags[0].Indices.EndIndex);
        Assert.AreEqual(0, basictags[0].Indices.OrderAtIndex);

        text.SetText("<color=red>L<s>o<wave>r</color>em<b> ip<!show>sum d</b>olor sit amet.Postte</s>xt");
        yield return null;
        basictags = animator.BasicTags.TagsAt(2).ToList();
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(1, basictags.Count);
        Assert.AreEqual(2, basictags[0].Indices.StartIndex);
        Assert.AreEqual(-1, basictags[0].Indices.EndIndex);
        Assert.AreEqual(0, basictags[0].Indices.OrderAtIndex);

        var commandtags = writer.CommandTags.TagsAt(8).ToList();
        Assert.AreEqual(1, writer.CommandTags.Count);
        Assert.AreEqual(1, commandtags.Count);
        Assert.AreEqual(8, commandtags[0].Indices.StartIndex);
        Assert.AreEqual(-1, commandtags[0].Indices.EndIndex);
        Assert.AreEqual(0, commandtags[0].Indices.OrderAtIndex);

        text.SetText("<color=red><style=h3>L<s>o<wave>r</color>em<b> ip<!show>s<style=h3>um d</b>olor sit amet.Postte</s>xt");
        yield return null;
        basictags = animator.BasicTags.TagsAt(2).ToList();
        Assert.AreEqual(1, animator.BasicTags.Count);
        Assert.AreEqual(1, basictags.Count);
        Assert.AreEqual(2, basictags[0].Indices.StartIndex);
        Assert.AreEqual(-1, basictags[0].Indices.EndIndex);
        Assert.AreEqual(0, basictags[0].Indices.OrderAtIndex);

        commandtags = writer.CommandTags.TagsAt(8).ToList();
        Assert.AreEqual(1, writer.CommandTags.Count);
        Assert.AreEqual(1, commandtags.Count);
        Assert.AreEqual(8, commandtags[0].Indices.StartIndex);
        Assert.AreEqual(-1, commandtags[0].Indices.EndIndex);
        Assert.AreEqual(0, commandtags[0].Indices.OrderAtIndex);

        yield break;
    }

    [UnityTest]
    public IEnumerator IndexAdjustmentTest_IndexTag()
    {
        text.SetText("Lorem <!delay=2>ipsum dol<!wait=1>or sit amet.");
        yield return null;
        var commandtags = writer.CommandTags.TagsAt(6).ToList();
        commandtags.AddRange(writer.CommandTags.TagsAt(15));
        Assert.AreEqual(2, writer.CommandTags.Count);
        Assert.AreEqual(2, commandtags.Count);
        Assert.AreEqual(6, commandtags[0].Indices.StartIndex);
        Assert.AreEqual(6, commandtags[0].Indices.EndIndex);
        Assert.IsTrue(commandtags[0].Indices.IsEmpty);
        Assert.AreEqual(0, commandtags[0].Indices.OrderAtIndex);
        Assert.AreEqual(15, commandtags[1].Indices.StartIndex);
        Assert.AreEqual(15, commandtags[1].Indices.EndIndex);
        Assert.IsTrue(commandtags[1].Indices.IsEmpty);
        Assert.AreEqual(0, commandtags[1].Indices.OrderAtIndex);

        text.SetText("<color=red>Lorem <!delay=2></color>ip<s>su<b>m d<color=blue></color>ol<!wait=1>or s</s>it amet.");
        yield return null;
        commandtags = writer.CommandTags.TagsAt(6).ToList();
        commandtags.AddRange(writer.CommandTags.TagsAt(15));
        Assert.AreEqual(2, writer.CommandTags.Count);
        Assert.AreEqual(2, commandtags.Count);
        Assert.AreEqual(6, commandtags[0].Indices.StartIndex);
        Assert.AreEqual(6, commandtags[0].Indices.EndIndex);
        Assert.IsTrue(commandtags[0].Indices.IsEmpty);
        Assert.AreEqual(0, commandtags[0].Indices.OrderAtIndex);
        Assert.AreEqual(15, commandtags[1].Indices.StartIndex);
        Assert.AreEqual(15, commandtags[1].Indices.EndIndex);
        Assert.IsTrue(commandtags[1].Indices.IsEmpty);
        Assert.AreEqual(0, commandtags[1].Indices.OrderAtIndex);

        text.SetText("<color=red>Lo<style=h3>rem <!delay=2></color>ip<s>su<b>m</style> d<color=blue><style=h3></color>ol<!wait=1>or s</s>it amet.");
        yield return null;
        commandtags = writer.CommandTags.TagsAt(6).ToList();
        commandtags.AddRange(writer.CommandTags.TagsAt(15));
        Assert.AreEqual(2, writer.CommandTags.Count);
        Assert.AreEqual(2, commandtags.Count);
        Assert.AreEqual(6, commandtags[0].Indices.StartIndex);
        Assert.AreEqual(6, commandtags[0].Indices.EndIndex);
        Assert.IsTrue(commandtags[0].Indices.IsEmpty);
        Assert.AreEqual(0, commandtags[0].Indices.OrderAtIndex);
        Assert.AreEqual(15, commandtags[1].Indices.StartIndex);
        Assert.AreEqual(15, commandtags[1].Indices.EndIndex);
        Assert.IsTrue(commandtags[1].Indices.IsEmpty);
        Assert.AreEqual(0, commandtags[1].Indices.OrderAtIndex);

        yield break;
    }

    [UnityTest]
    public IEnumerator IndexAdjustmentTest()
    {
        text.SetText("Lorem<wave><pivot> i<!delay=2><!delay=2>psu<shake>m</> <?event>dolor</wave> sit amet.");
        yield return null;
        var eventtags = writer.EventTags.TagsAt(12).ToList();
        Assert.AreEqual(1, writer.EventTags.Count);
        Assert.AreEqual(1, eventtags.Count);
        Assert.AreEqual(12, eventtags[0].Indices.StartIndex);
        Assert.AreEqual(12, eventtags[0].Indices.EndIndex);
        Assert.IsTrue(eventtags[0].Indices.IsEmpty);
        Assert.AreEqual(0, eventtags[0].Indices.OrderAtIndex);

        var commandtags = writer.CommandTags.TagsAt(7).ToList();
        Assert.AreEqual(2, writer.CommandTags.Count);
        Assert.AreEqual(2, commandtags.Count);
        Assert.AreEqual(7, commandtags[0].Indices.StartIndex);
        Assert.AreEqual(7, commandtags[0].Indices.EndIndex);
        Assert.IsTrue(commandtags[0].Indices.IsEmpty);
        Assert.AreEqual(7, commandtags[1].Indices.StartIndex);
        Assert.AreEqual(7, commandtags[1].Indices.EndIndex);
        Assert.IsTrue(commandtags[1].Indices.IsEmpty);
        Assert.AreEqual(0, commandtags[0].Indices.OrderAtIndex);
        Assert.AreEqual(1, commandtags[1].Indices.OrderAtIndex);

        var basictags = animator.BasicTags.TagsAt(5).ToList();
        basictags.AddRange(animator.BasicTags.TagsAt(10));
        Assert.AreEqual(3, animator.BasicTags.Count);
        Assert.AreEqual(3, basictags.Count);
        Assert.AreEqual(5, basictags[0].Indices.StartIndex);
        Assert.AreEqual(17, basictags[0].Indices.EndIndex);
        Assert.AreEqual(0, basictags[0].Indices.OrderAtIndex);
        Assert.AreEqual(5, basictags[1].Indices.StartIndex);
        Assert.AreEqual(-1, basictags[1].Indices.EndIndex);
        Assert.AreEqual(1, basictags[1].Indices.OrderAtIndex);
        Assert.AreEqual(10, basictags[2].Indices.StartIndex);
        Assert.AreEqual(11, basictags[2].Indices.EndIndex);
        Assert.AreEqual(0, basictags[2].Indices.OrderAtIndex);


        text.SetText("<color=red>Lorem<!delay=2></color>ipsumdolorsitamet.");
        yield return null;
        commandtags = writer.CommandTags.TagsAt(5).ToList();
        Assert.AreEqual(1, writer.CommandTags.Count);
        Assert.AreEqual(1, commandtags.Count);
        Assert.AreEqual(5, commandtags[0].Indices.StartIndex);
        Assert.AreEqual(5, commandtags[0].Indices.EndIndex);
        Assert.IsTrue(commandtags[0].Indices.IsEmpty);
        Assert.AreEqual(0, commandtags[0].Indices.OrderAtIndex);

        yield break;
    }

    private void SetUp_Components(Scene scene, LoadSceneMode loadSceneMode)
    {
        EditorSceneManager.sceneLoaded -= SetUp_Components;

        animator = GameObject.FindObjectOfType<TMPAnimator>();
        writer = GameObject.FindObjectOfType<TMPWriter>();
        text = GameObject.FindObjectOfType<TMP_Text>();

        if (animator == null) Debug.LogWarning("ANIMATOR NULL");
        if (writer == null) Debug.LogWarning("WRITER NULL");
        if (text == null) Debug.LogWarning("TEXT NULL");

        animationTagProcessor = new TagProcessor(new TMPAnimationCategory(TMPAnimator.ANIMATION_PREFIX, animator.Database.BasicAnimationDatabase));
        showAnimationTagProcessor = new TagProcessor(new TMPAnimationCategory(TMPAnimator.SHOW_ANIMATION_PREFIX, animator.Database.ShowAnimationDatabase));
        hideAnimationTagProcessor = new TagProcessor(new TMPAnimationCategory(TMPAnimator.HIDE_ANIMATION_PREFIX, animator.Database.HideAnimationDatabase));
        commandTagProcessor = new TagProcessor(new TMPCommandCategory(TMPWriter.COMMAND_PREFIX, writer.Database));
        eventTagProcessor = new TagProcessor(new TMPEventCategory(TMPWriter.EVENT_PREFIX));
    }
}
