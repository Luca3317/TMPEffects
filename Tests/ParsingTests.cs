using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ParsingTests
{
    const string str1 = "Some string <tag> with a few <tag2> tags!";
    const string str2 = "Some string <> with a few faulty <tag2 tags (and a last correct <tag3> one)!";
    const string str3 = "Some string <tag> with </tag> closing tags!";

    [Test]
    public void IsTagParsingTests()
    {
        Assert.AreEqual("<tag>", str1.Substring(12, 5));
        Assert.IsTrue(ParsingUtility.IsTag(str1, 12));

        Assert.AreEqual("<tag2>", str1.Substring(29, 6));
        Assert.IsTrue(ParsingUtility.IsTag(str1, 29));

        Assert.AreEqual("<>", str2.Substring(12, 2));
        Assert.IsFalse(ParsingUtility.IsTag(str2, 12));

        Assert.AreEqual("<tag2 tags (and a last correct <tag3> one)!", str2.Substring(33, str2.Length - 33));
        Assert.IsFalse(ParsingUtility.IsTag(str2, 33));

        Assert.AreEqual("<tag3> one)!", str2.Substring(64, str2.Length - 64));
        Assert.IsTrue(ParsingUtility.IsTag(str2, 64));


        Assert.AreEqual("<tag>", str1.Substring(12, 5));
        Assert.IsFalse(ParsingUtility.IsTag(str1, 12, type: ParsingUtility.TagType.Close));

        Assert.AreEqual("<tag2>", str1.Substring(29, 6));
        Assert.IsFalse(ParsingUtility.IsTag(str1, 29, type: ParsingUtility.TagType.Close));

        Assert.AreEqual("<>", str2.Substring(12, 2));
        Assert.IsFalse(ParsingUtility.IsTag(str2, 12, type: ParsingUtility.TagType.Close));

        Assert.AreEqual("<tag2 tags (and a last correct <tag3> one)!", str2.Substring(33, str2.Length - 33));
        Assert.IsFalse(ParsingUtility.IsTag(str2, 33, type: ParsingUtility.TagType.Close));

        Assert.AreEqual("<tag3> one)!", str2.Substring(64, str2.Length - 64));
        Assert.IsFalse(ParsingUtility.IsTag(str2, 64, type: ParsingUtility.TagType.Close));


        Assert.AreEqual("</tag>", str3.Substring(23, 6));
        Assert.IsTrue(ParsingUtility.IsTag(str3, 23, type: ParsingUtility.TagType.Close));
        Assert.IsFalse(ParsingUtility.IsTag(str3, 23, type: ParsingUtility.TagType.Open));
    }

    [Test]
    public void GetNextTagTests()
    {
        string tag;

        ParsingUtility.GetNextTag(str1, 0, out tag);
        Assert.AreEqual("<tag>", tag);

        ParsingUtility.GetNextTag(str1, 15, out tag);
        Assert.AreEqual("<tag2>", tag);

        ParsingUtility.GetNextTag(str2, 0, out tag);
        Assert.AreEqual("<tag3>", tag);

        ParsingUtility.GetNextTag(str3, 0, out tag);
        Assert.AreEqual("<tag>", tag);

        ParsingUtility.GetNextTag(str3, 15, out tag);
        Assert.AreEqual("</tag>", tag);

        ParsingUtility.GetNextTag(str3, 0, out tag, ParsingUtility.TagType.Close);
        Assert.AreEqual("</tag>", tag);

        ParsingUtility.GetNextTag(str1, 0, out tag, ParsingUtility.TagType.Close);
        Assert.AreEqual("", tag);

        ParsingUtility.GetNextTag(str7, "tag", 0, out tag);
        Assert.AreEqual("<tag>", tag);

        ParsingUtility.GetNextTag(str7, "tag1", 0, out tag);
        Assert.AreEqual("<tag1>", tag);

        ParsingUtility.GetNextTag(str7, "tag2", 0, out tag);
        Assert.AreEqual("<tag2>", tag);
    }

    const string str4 = "Some string <tag=value> with values!";
    const string str5 = "Some string <tag key=value> with attributes!";
    const string str6 = "Some string <tag=value key1=value1 key2=value2> with both!";
    const string str7 = "Some string <tag> with <tag1> various <tag2> tags <tag1>!";

    [Test]
    public void GetTagParamsTests()
    {
        string tag; 

        ParsingUtility.GetNextTag(str4, 0, out tag);
        Assert.AreEqual("<tag=value>", tag);
        Assert.AreEqual("tag", ParsingUtility.GetTagName(tag));
        Assert.AreEqual("=value", ParsingUtility.GetTagParameters(tag));

        ParsingUtility.GetNextTag(str5, 0, out tag);
        Assert.AreEqual("<tag key=value>", tag);
        Assert.AreEqual("tag", ParsingUtility.GetTagName(tag));
        Assert.AreEqual("key=value", ParsingUtility.GetTagParameters(tag));

        ParsingUtility.GetNextTag(str6, 0, out tag);
        Assert.AreEqual("<tag=value key1=value1 key2=value2>", tag);
        Assert.AreEqual("tag", ParsingUtility.GetTagName(tag));
        Assert.AreEqual("=value key1=value1 key2=value2", ParsingUtility.GetTagParameters(tag));
    }

    [Test]
    public void GetTagValuesTests()
    {
        string tag;

        ParsingUtility.GetNextTag(str4, 0, out tag);
        Dictionary<string,string> dict =  ParsingUtility.GetTagParametersDict(tag);
        Assert.IsTrue(dict.ContainsKey("tag"));
        Assert.AreEqual("value", dict["tag"]);
        Assert.AreEqual(1, dict.Count);

        ParsingUtility.GetNextTag(str5, 0, out tag);
        dict = ParsingUtility.GetTagParametersDict(tag);
        Assert.IsTrue(dict.ContainsKey("key"));
        Assert.AreEqual("value", dict["key"]);
        Assert.IsTrue(dict.ContainsKey("tag"));
        Assert.AreEqual("", dict["tag"]);
        Assert.AreEqual(2, dict.Count);

        ParsingUtility.GetNextTag(str6, 0, out tag);
        dict = ParsingUtility.GetTagParametersDict(tag);
        Assert.IsTrue(dict.ContainsKey("tag"));
        Assert.AreEqual("value", dict["tag"]);
        Assert.IsTrue(dict.ContainsKey("key1"));
        Assert.AreEqual("value1", dict["key1"]); 
        Assert.IsTrue(dict.ContainsKey("key2"));
        Assert.AreEqual("value2", dict["key2"]);
        Assert.AreEqual(3, dict.Count);

        ParsingUtility.GetNextTag(str3, 0, out tag);
        dict = ParsingUtility.GetTagParametersDict(tag);
        Assert.IsTrue(dict.ContainsKey("tag"));
        Assert.AreEqual("", dict["tag"]);
        Assert.AreEqual(1, dict.Count);
    }

    //// A Test behaves as an ordinary method
    //[Test]
    //public void ParsingTestsSimplePasses()
    //{
    //    // Use the Assert class to test conditions
    //}

    //// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    //// `yield return null;` to skip a frame.
    //[UnityTest]
    //public IEnumerator ParsingTestsWithEnumeratorPasses()
    //{
    //    // Use the Assert class to test conditions.
    //    // Use yield to skip a frame.
    //    yield return null;
    //}
}
