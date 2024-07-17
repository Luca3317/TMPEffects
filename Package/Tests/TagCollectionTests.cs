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
using TMPEffects.Tags;
using System.Collections.Generic;
using System.Data.Common;
using UnityEditor;

public class TagCollectionTests
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
    public IEnumerator ProcessorCollectionTest()
    {
        animator.SetText("<wave>L<!show>or<+move>e</!>m ip<-move><!delay=2>su</shake>m do<?event>lo<-char>r </+move>si<pivot><shake>t amet. Lorem i<!wait=2>psu<char>m d</pivot>olor si<?event2>t am<!delay=2>et.");

        yield return null;

        Assert.AreEqual(4, animator.BasicTags.Count);
        Assert.AreEqual(1, animator.ShowTags.Count);
        Assert.AreEqual(2, animator.HideTags.Count);
        Assert.AreEqual(4, writer.CommandTags.Count);
        Assert.AreEqual(2, writer.EventTags.Count);

        Assert.AreEqual(1, animator.BasicTags.TagsAt(0).Count());
        Assert.AreEqual(2, animator.BasicTags.TagsAt(28).Count());
        Assert.AreEqual(1, animator.BasicTags.TagsAt(46).Count());

        Assert.AreEqual(1, animator.ShowTags.TagsAt(3).Count());

        Assert.AreEqual(1, animator.HideTags.TagsAt(8).Count());
        Assert.AreEqual(1, animator.HideTags.TagsAt(24).Count());

        Assert.AreEqual(1, writer.CommandTags.TagsAt(1).Count());
        Assert.AreEqual(1, writer.CommandTags.TagsAt(8).Count());
        Assert.AreEqual(1, writer.CommandTags.TagsAt(43).Count());
        Assert.AreEqual(1, writer.CommandTags.TagsAt(60).Count());

        Assert.AreEqual(1, writer.EventTags.TagsAt(22).Count());
        Assert.AreEqual(1, writer.EventTags.TagsAt(56).Count());

        yield break;
    }

    [UnityTest, Order(0)]
    public IEnumerator InsertionTest()
    {
        animator.SetText("<wave>L<!show>or<+move>e</!>m ip<-move><!delay=2>su</shake>m do<?event>lo<-char>r </+move>si<pivot><shake>t amet. Lorem i<!wait=2>psu<char>m d</pivot>olor si<?event2>t am<!delay=2>et.");

        yield return null;

        Assert.AreEqual(0, animator.BasicTags.TagsAt(28).ToList()[0].Indices.OrderAtIndex);
        Assert.AreEqual(1, animator.BasicTags.TagsAt(28).ToList()[1].Indices.OrderAtIndex);
        Assert.AreEqual("pivot", animator.BasicTags.TagsAt(28).ToList()[0].Tag.Name);
        Assert.AreEqual("shake", animator.BasicTags.TagsAt(28).ToList()[1].Tag.Name);

        Assert.True(animator.BasicTags.TryAdd(new TMPEffectTag("wave", '\0', new Dictionary<string, string>()), 28, 30));
        Assert.AreEqual(-1, animator.BasicTags.TagsAt(28).ToList()[0].Indices.OrderAtIndex);
        Assert.AreEqual(0, animator.BasicTags.TagsAt(28).ToList()[1].Indices.OrderAtIndex);
        Assert.AreEqual(1, animator.BasicTags.TagsAt(28).ToList()[2].Indices.OrderAtIndex);
        Assert.AreEqual("wave", animator.BasicTags.TagsAt(28).ToList()[0].Tag.Name);
        Assert.AreEqual("pivot", animator.BasicTags.TagsAt(28).ToList()[1].Tag.Name);
        Assert.AreEqual("shake", animator.BasicTags.TagsAt(28).ToList()[2].Tag.Name);

        Assert.True(animator.BasicTags.TryAdd(new TMPEffectTag("funky", '\0', new Dictionary<string, string>()), 28, 30, 0));
        Assert.AreEqual(-1, animator.BasicTags.TagsAt(28).ToList()[0].Indices.OrderAtIndex);
        Assert.AreEqual(0, animator.BasicTags.TagsAt(28).ToList()[1].Indices.OrderAtIndex);
        Assert.AreEqual(1, animator.BasicTags.TagsAt(28).ToList()[2].Indices.OrderAtIndex);
        Assert.AreEqual(2, animator.BasicTags.TagsAt(28).ToList()[3].Indices.OrderAtIndex);
        Assert.AreEqual("wave", animator.BasicTags.TagsAt(28).ToList()[0].Tag.Name);
        Assert.AreEqual("funky", animator.BasicTags.TagsAt(28).ToList()[1].Tag.Name);
        Assert.AreEqual("pivot", animator.BasicTags.TagsAt(28).ToList()[2].Tag.Name);
        Assert.AreEqual("shake", animator.BasicTags.TagsAt(28).ToList()[3].Tag.Name);

        Assert.True(animator.BasicTags.TryAdd(new TMPEffectTag("char", '\0', new Dictionary<string, string>()), 28, 30, 1));
        Assert.AreEqual(-1, animator.BasicTags.TagsAt(28).ToList()[0].Indices.OrderAtIndex);
        Assert.AreEqual(0, animator.BasicTags.TagsAt(28).ToList()[1].Indices.OrderAtIndex);
        Assert.AreEqual(1, animator.BasicTags.TagsAt(28).ToList()[2].Indices.OrderAtIndex);
        Assert.AreEqual(2, animator.BasicTags.TagsAt(28).ToList()[3].Indices.OrderAtIndex);
        Assert.AreEqual(3, animator.BasicTags.TagsAt(28).ToList()[4].Indices.OrderAtIndex);
        Assert.AreEqual("wave", animator.BasicTags.TagsAt(28).ToList()[0].Tag.Name);
        Assert.AreEqual("funky", animator.BasicTags.TagsAt(28).ToList()[1].Tag.Name);
        Assert.AreEqual("char", animator.BasicTags.TagsAt(28).ToList()[2].Tag.Name);
        Assert.AreEqual("pivot", animator.BasicTags.TagsAt(28).ToList()[3].Tag.Name);
        Assert.AreEqual("shake", animator.BasicTags.TagsAt(28).ToList()[4].Tag.Name);

        Assert.AreEqual(0, writer.EventTags.TagsAt(22).ToList()[0].Indices.OrderAtIndex);
        Assert.AreEqual("event", writer.EventTags.TagsAt(22).ToList()[0].Tag.Name);

        Assert.True(writer.EventTags.TryAdd(new TMPEffectTag("event2", '?', new Dictionary<string, string>()), 22, 22, 1));
        Assert.True(writer.CommandTags.TryAdd(new TMPEffectTag("show", '!', new Dictionary<string, string>()), 22, 30, 1));
        Assert.AreEqual(0, writer.Tags.TagsAt(22).ToList()[0].Indices.OrderAtIndex);
        Assert.AreEqual(1, writer.Tags.TagsAt(22).ToList()[1].Indices.OrderAtIndex);
        Assert.AreEqual(2, writer.Tags.TagsAt(22).ToList()[2].Indices.OrderAtIndex);
        Assert.AreEqual("event", writer.Tags.TagsAt(22).ToList()[0].Tag.Name);
        Assert.AreEqual("show", writer.Tags.TagsAt(22).ToList()[1].Tag.Name);
        Assert.AreEqual("event2", writer.Tags.TagsAt(22).ToList()[2].Tag.Name);

        Assert.True(writer.EventTags.TryAdd(new TMPEffectTag("event3", '?', new Dictionary<string, string>()), 22, 22));
        Assert.AreEqual(-1, writer.Tags.TagsAt(22).ToList()[0].Indices.OrderAtIndex);
        Assert.AreEqual(0, writer.Tags.TagsAt(22).ToList()[1].Indices.OrderAtIndex);
        Assert.AreEqual(1, writer.Tags.TagsAt(22).ToList()[2].Indices.OrderAtIndex);
        Assert.AreEqual(2, writer.Tags.TagsAt(22).ToList()[3].Indices.OrderAtIndex);
        Assert.AreEqual("event3", writer.Tags.TagsAt(22).ToList()[0].Tag.Name);
        Assert.AreEqual("event", writer.Tags.TagsAt(22).ToList()[1].Tag.Name);
        Assert.AreEqual("show", writer.Tags.TagsAt(22).ToList()[2].Tag.Name);
        Assert.AreEqual("event2", writer.Tags.TagsAt(22).ToList()[3].Tag.Name);

        Assert.True(animator.ShowTags.TryAdd(new TMPEffectTag("move", '+', new Dictionary<string, string>()), new TMPEffectTagIndices(28, 30, 2)));
        Assert.AreEqual(-1, animator.Tags.TagsAt(28).ToList()[0].Indices.OrderAtIndex);
        Assert.AreEqual(0, animator.Tags.TagsAt(28).ToList()[1].Indices.OrderAtIndex);
        Assert.AreEqual(1, animator.Tags.TagsAt(28).ToList()[2].Indices.OrderAtIndex);
        Assert.AreEqual(2, animator.Tags.TagsAt(28).ToList()[3].Indices.OrderAtIndex);
        Assert.AreEqual(3, animator.Tags.TagsAt(28).ToList()[4].Indices.OrderAtIndex);
        Assert.AreEqual(4, animator.Tags.TagsAt(28).ToList()[5].Indices.OrderAtIndex);
        Assert.AreEqual("wave", animator.Tags.TagsAt(28).ToList()[0].Tag.Name);
        Assert.AreEqual("funky", animator.Tags.TagsAt(28).ToList()[1].Tag.Name);
        Assert.AreEqual("char", animator.Tags.TagsAt(28).ToList()[2].Tag.Name);
        Assert.AreEqual("move", animator.Tags.TagsAt(28).ToList()[3].Tag.Name);
        Assert.AreEqual("pivot", animator.Tags.TagsAt(28).ToList()[4].Tag.Name);
        Assert.AreEqual("shake", animator.Tags.TagsAt(28).ToList()[5].Tag.Name);

        yield break;
    }

    [UnityTest, Order(1)]
    public IEnumerator RemovalTest()
    {
        animator.SetText("<wave>L<!show>or<+move>e</!>m ip<-move><!delay=2>su</shake>m do<?event>lo<-char>r </+move>si<pivot><shake>t amet. Lorem i<!wait=2>psu<char>m d</pivot>olor si<?event2>t am<!delay=2>et.");
        yield return null;
        Assert.True(animator.BasicTags.TryAdd(new TMPEffectTag("wave", '\0', new Dictionary<string, string>()), 28, 30));
        Assert.True(animator.BasicTags.TryAdd(new TMPEffectTag("funky", '\0', new Dictionary<string, string>()), 28, 30, 0));
        Assert.True(animator.BasicTags.TryAdd(new TMPEffectTag("char", '\0', new Dictionary<string, string>()), 28, 30, 1));
        Assert.True(writer.EventTags.TryAdd(new TMPEffectTag("event2", '?', new Dictionary<string, string>()), 22, 22, 1));
        Assert.True(writer.CommandTags.TryAdd(new TMPEffectTag("show", '!', new Dictionary<string, string>()), 22, 30, 1));
        Assert.True(writer.EventTags.TryAdd(new TMPEffectTag("event3", '?', new Dictionary<string, string>()), 22, 22));
        Assert.True(animator.ShowTags.TryAdd(new TMPEffectTag("move", '+', new Dictionary<string, string>()), new TMPEffectTagIndices(28, 30, 2)));

        Assert.AreEqual(-1, animator.Tags.TagsAt(28).ToList()[0].Indices.OrderAtIndex);
        Assert.AreEqual(0, animator.Tags.TagsAt(28).ToList()[1].Indices.OrderAtIndex);
        Assert.AreEqual(1, animator.Tags.TagsAt(28).ToList()[2].Indices.OrderAtIndex);
        Assert.AreEqual(2, animator.Tags.TagsAt(28).ToList()[3].Indices.OrderAtIndex);
        Assert.AreEqual(3, animator.Tags.TagsAt(28).ToList()[4].Indices.OrderAtIndex);
        Assert.AreEqual(4, animator.Tags.TagsAt(28).ToList()[5].Indices.OrderAtIndex);
        Assert.AreEqual("wave", animator.Tags.TagsAt(28).ToList()[0].Tag.Name);
        Assert.AreEqual("funky", animator.Tags.TagsAt(28).ToList()[1].Tag.Name);
        Assert.AreEqual("char", animator.Tags.TagsAt(28).ToList()[2].Tag.Name);
        Assert.AreEqual("move", animator.Tags.TagsAt(28).ToList()[3].Tag.Name);
        Assert.AreEqual("pivot", animator.Tags.TagsAt(28).ToList()[4].Tag.Name);
        Assert.AreEqual("shake", animator.Tags.TagsAt(28).ToList()[5].Tag.Name);

        Assert.True(animator.BasicTags.Remove(animator.BasicTags.TagsAt(28).ToList()[2].Tag, animator.BasicTags.TagsAt(28).ToList()[2].Indices));
        Assert.AreEqual(-1, animator.Tags.TagsAt(28).ToList()[0].Indices.OrderAtIndex);
        Assert.AreEqual(0, animator.Tags.TagsAt(28).ToList()[1].Indices.OrderAtIndex);
        Assert.AreEqual(2, animator.Tags.TagsAt(28).ToList()[2].Indices.OrderAtIndex);
        Assert.AreEqual(3, animator.Tags.TagsAt(28).ToList()[3].Indices.OrderAtIndex);
        Assert.AreEqual(4, animator.Tags.TagsAt(28).ToList()[4].Indices.OrderAtIndex);
        Assert.AreEqual("wave", animator.Tags.TagsAt(28).ToList()[0].Tag.Name);
        Assert.AreEqual("funky", animator.Tags.TagsAt(28).ToList()[1].Tag.Name);
        Assert.AreEqual("move", animator.Tags.TagsAt(28).ToList()[2].Tag.Name);
        Assert.AreEqual("pivot", animator.Tags.TagsAt(28).ToList()[3].Tag.Name);
        Assert.AreEqual("shake", animator.Tags.TagsAt(28).ToList()[4].Tag.Name);

        Assert.True(writer.EventTags.Remove(writer.EventTags.TagsAt(22).ToList()[1].Tag, writer.EventTags.TagsAt(22).ToList()[1].Indices));
        Assert.AreEqual(-1, writer.Tags.TagsAt(22).ToList()[0].Indices.OrderAtIndex);
        Assert.AreEqual(1, writer.Tags.TagsAt(22).ToList()[1].Indices.OrderAtIndex);
        Assert.AreEqual(2, writer.Tags.TagsAt(22).ToList()[2].Indices.OrderAtIndex);
        Assert.AreEqual("event3", writer.Tags.TagsAt(22).ToList()[0].Tag.Name);
        Assert.AreEqual("show", writer.Tags.TagsAt(22).ToList()[1].Tag.Name);
        Assert.AreEqual("event2", writer.Tags.TagsAt(22).ToList()[2].Tag.Name);

        writer.Tags.Clear();
        Assert.AreEqual(0, writer.Tags.Count);
        Assert.AreEqual(0, writer.EventTags.Count);
        Assert.AreEqual(0, writer.CommandTags.Count);

        Assert.AreEqual(5, animator.Tags.TagsAt(28).Count());
        Assert.AreEqual(4, animator.BasicTags.TagsAt(28).Count());
        Assert.AreEqual(1, animator.ShowTags.TagsAt(28).Count());
        animator.Tags.RemoveAllAt(28);
        Assert.AreEqual(0, animator.Tags.TagsAt(28).Count());
        Assert.AreEqual(0, animator.BasicTags.TagsAt(28).Count());
        Assert.AreEqual(0, animator.ShowTags.TagsAt(28).Count());

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
