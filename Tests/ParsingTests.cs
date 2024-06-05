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

public class ParsingTests
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

    [Test]
    public void ParsingTagTest()
    {
        ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

        Assert.AreEqual(true, ParsingUtility.TryParseTag(testStr4, 0, 5, ref tagInfo));
        Assert.AreEqual("wave", tagInfo.name);
        Assert.AreEqual('\0', tagInfo.prefix);
        Assert.AreEqual(0, tagInfo.startIndex);
        Assert.AreEqual("wave", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Open, tagInfo.type);

        string str = "<!wave amp=10 upcrv=easeinoutsine>";
        Assert.AreEqual(true, ParsingUtility.TryParseTag(str, 0, str.Length - 1, ref tagInfo));
        Assert.AreEqual("wave", tagInfo.name);
        Assert.AreEqual('!', tagInfo.prefix);
        Assert.AreEqual(0, tagInfo.startIndex);
        Assert.AreEqual("wave amp=10 upcrv=easeinoutsine", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Open, tagInfo.type);

        str = "</!wave amp=10 upcrv=easeinoutsine>";
        Assert.AreEqual(true, ParsingUtility.TryParseTag(str, 0, str.Length - 1, ref tagInfo));
        Assert.AreEqual("wave", tagInfo.name);
        Assert.AreEqual('!', tagInfo.prefix);
        Assert.AreEqual(0, tagInfo.startIndex);
        Assert.AreEqual("wave amp=10 upcrv=easeinoutsine", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Close, tagInfo.type);

        str = "!wave amp=10 upcrv=easeinoutsine>";
        Assert.AreEqual(false, ParsingUtility.TryParseTag(str, 0, str.Length - 1, ref tagInfo));
        str = "<!wave amp=10 upcrv=easeinoutsine";
        Assert.AreEqual(false, ParsingUtility.TryParseTag(str, 0, str.Length - 1, ref tagInfo));
        str = "<! amp=10 upcrv=easeinoutsine>";
        Assert.AreEqual(false, ParsingUtility.TryParseTag(str, 0, str.Length - 1, ref tagInfo));
    }

    [Test]
    public void GetNextTagTest()
    {
        int index = 0;
        ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("wave", tagInfo.name);
        Assert.AreEqual('\0', tagInfo.prefix);
        Assert.AreEqual(0, tagInfo.startIndex);
        Assert.AreEqual("wave", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Open, tagInfo.type);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("wait", tagInfo.name);
        Assert.AreEqual('!', tagInfo.prefix);
        Assert.AreEqual(18, tagInfo.startIndex);
        Assert.AreEqual("wait=2", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Open, tagInfo.type);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("", tagInfo.name);
        Assert.AreEqual('\0', tagInfo.prefix);
        Assert.AreEqual(41, tagInfo.startIndex);
        Assert.AreEqual("", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Close, tagInfo.type);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("myEvent", tagInfo.name);
        Assert.AreEqual('?', tagInfo.prefix);
        Assert.AreEqual(58, tagInfo.startIndex);
        Assert.AreEqual("myEvent=myValue secondKey=secondValue", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Open, tagInfo.type);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("jump", tagInfo.name);
        Assert.AreEqual('\0', tagInfo.prefix);
        Assert.AreEqual(115, tagInfo.startIndex);
        Assert.AreEqual("jump amp=10", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Open, tagInfo.type);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("delay", tagInfo.name);
        Assert.AreEqual('!', tagInfo.prefix);
        Assert.AreEqual(134, tagInfo.startIndex);
        Assert.AreEqual("delay=4", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Open, tagInfo.type);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("", tagInfo.name);
        Assert.AreEqual('\0', tagInfo.prefix);
        Assert.AreEqual(183, tagInfo.startIndex);
        Assert.AreEqual("", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Close, tagInfo.type);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("show", tagInfo.name);
        Assert.AreEqual('!', tagInfo.prefix);
        Assert.AreEqual(189, tagInfo.startIndex);
        Assert.AreEqual("show", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Open, tagInfo.type);
        index = tagInfo.endIndex;

        //Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        //Assert.AreEqual("", tagInfo.name);
        //Assert.AreEqual('!', tagInfo.prefix);
        //Assert.AreEqual(189, tagInfo.startIndex);
        //Assert.AreEqual("show", tagInfo.parameterString);
        //Assert.AreEqual(ParsingUtility.TagType.Open, tagInfo.type);
        //index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("wave", tagInfo.name);
        Assert.AreEqual('\0', tagInfo.prefix);
        Assert.AreEqual(197, tagInfo.startIndex);
        Assert.AreEqual("wave", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Open, tagInfo.type);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("wave", tagInfo.name);
        Assert.AreEqual('\0', tagInfo.prefix);
        Assert.AreEqual(204, tagInfo.startIndex);
        Assert.AreEqual("wave", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Close, tagInfo.type);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("wave", tagInfo.name);
        Assert.AreEqual('\0', tagInfo.prefix);
        Assert.AreEqual(211, tagInfo.startIndex);
        Assert.AreEqual("wave", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Open, tagInfo.type);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("", tagInfo.name);
        Assert.AreEqual('\0', tagInfo.prefix);
        Assert.AreEqual(217, tagInfo.startIndex);
        Assert.AreEqual("", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Close, tagInfo.type);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        Assert.AreEqual("", tagInfo.name);
        Assert.AreEqual('!', tagInfo.prefix);
        Assert.AreEqual(221, tagInfo.startIndex);
        Assert.AreEqual("", tagInfo.parameterString);
        Assert.AreEqual(ParsingUtility.TagType.Close, tagInfo.type);
        index = tagInfo.endIndex;

        Assert.AreEqual(false, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
    }

    [Test]
    public void ParsingParametersTest()
    {
        int index = 0;
        ParsingUtility.TagInfo tagInfo = new ParsingUtility.TagInfo();

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        var parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
        Assert.NotNull(parameters);
        Assert.True(parameters.Keys.Count == 1);
        Assert.True(parameters.ContainsKey(""));
        Assert.AreEqual("", parameters[""]);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
        Assert.NotNull(parameters);
        Assert.True(parameters.Keys.Count == 1);
        Assert.True(parameters.ContainsKey(""));
        Assert.AreEqual("2", parameters[""]);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
        Assert.NotNull(parameters);
        Assert.True(parameters.Keys.Count == 1);
        Assert.True(parameters.ContainsKey(""));
        Assert.AreEqual("", parameters[""]);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
        Assert.NotNull(parameters);
        Assert.True(parameters.Keys.Count == 2);
        Assert.True(parameters.ContainsKey(""));
        Assert.True(parameters.ContainsKey("secondKey"));
        Assert.AreEqual("myValue", parameters[""]);
        Assert.AreEqual("secondValue", parameters["secondKey"]);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
        Assert.NotNull(parameters);
        Assert.True(parameters.Keys.Count == 2);
        Assert.True(parameters.ContainsKey(""));
        Assert.True(parameters.ContainsKey("amp"));
        Assert.AreEqual("10", parameters["amp"]);
        Assert.AreEqual("", parameters[""]);
        index = tagInfo.endIndex;

        Assert.AreEqual(true, ParsingUtility.GetNextTag(testStr4, index, ref tagInfo));
        parameters = ParsingUtility.GetTagParametersDict(tagInfo.parameterString);
        Assert.NotNull(parameters);
        Assert.True(parameters.Keys.Count == 1);
        Assert.True(parameters.ContainsKey(""));
        Assert.AreEqual("4", parameters[""]);
        index = tagInfo.endIndex;

        parameters = ParsingUtility.GetTagParametersDict("<!fake somekey=somevalue some2ndkey=\"some2nd value with spaces\" some3rdkey=some3rd key with spaces>");
        Assert.AreEqual(parameters, ParsingUtility.GetTagParametersDict("fake somekey=somevalue some2ndkey=\"some2nd value with spaces\" some3rdkey=some3rd key with spaces"));
        Assert.NotNull(parameters);
        Assert.AreEqual(7, parameters.Keys.Count);
        Assert.True(parameters.ContainsKey(""));
        Assert.AreEqual("", parameters[""]);
        Assert.AreEqual("somevalue", parameters["somekey"]);
        Assert.AreEqual("some2nd value with spaces", parameters["some2ndkey"]);
        Assert.AreEqual("some3rd", parameters["some3rdkey"]);
        Assert.AreEqual("", parameters["key"]);
        Assert.AreEqual("", parameters["with"]);
        Assert.AreEqual("", parameters["spaces"]);

        parameters = ParsingUtility.GetTagParametersDict("");
        Assert.NotNull(parameters);
        Assert.AreEqual(1, parameters.Keys.Count);
        Assert.AreEqual("", parameters[""]);

        parameters = ParsingUtility.GetTagParametersDict("</>");
        Assert.NotNull(parameters);
        Assert.AreEqual(1, parameters.Keys.Count);
        Assert.AreEqual("", parameters[""]);

        parameters = ParsingUtility.GetTagParametersDict("</!>");
        Assert.NotNull(parameters);
        Assert.AreEqual(1, parameters.Keys.Count);
        Assert.AreEqual("", parameters[""]);

        parameters = ParsingUtility.GetTagParametersDict("<>");
        Assert.NotNull(parameters);
        Assert.AreEqual(1, parameters.Keys.Count);
        Assert.AreEqual("", parameters[""]);

        parameters = ParsingUtility.GetTagParametersDict("<!>");
        Assert.NotNull(parameters);
        Assert.AreEqual(1, parameters.Keys.Count);
        Assert.AreEqual("", parameters[""]);
    }

    [Test]
    public void ParsingIsTagTest()
    {
        Assert.True(ParsingUtility.IsTag("<wave>"));
        Assert.True(ParsingUtility.IsTag("<!wave>"));
        Assert.True(ParsingUtility.IsTag("<wave amp=10 upcrv=easein somestring=\"some string in here\">"));
        Assert.True(ParsingUtility.IsTag("<>"));

        Assert.True(ParsingUtility.IsTag("</wave>"));
        Assert.True(ParsingUtility.IsTag("</!wave>"));
        Assert.True(ParsingUtility.IsTag("</wave amp=10 upcrv=easein somestring=\"some string in here\">"));
        Assert.True(ParsingUtility.IsTag("</>"));

        Assert.False(ParsingUtility.IsTag("wave"));
        Assert.False(ParsingUtility.IsTag("!wave"));
        Assert.False(ParsingUtility.IsTag("wave amp=10 upcrv=easein somestring=\"some string in here\""));
        Assert.False(ParsingUtility.IsTag(""));
    }
}