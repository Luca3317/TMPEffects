using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PreprocessTests
{
    const string str1 = "Some <tag>text that is to be </tag>preprocessed";
    const string str2 = "Some <tag>that is <tag2><tag3>much</tag3> more nested than</tag2> the </tag>previous";
    const string str3 = "<tag3>Some <tag>text that<tag2> is </tag>even</tag3> more nested<tag4>, and in more </tag2>confusing ways</tag4>";


    [Test]
    public void TagRemovalTests()
    {
        
        TMProEffectPreprocessor preprocessor = new TMProEffectPreprocessor((TMPEffectDatabase)Resources.Load("DefaultTMPEffectsDatabase"));

        Assert.AreEqual("Some text that is to be preprocessed", preprocessor.PreprocessText(str1));
        Assert.AreEqual("Some that is much more nested than the previous", preprocessor.PreprocessText(str2));
        Assert.AreEqual("Some text that is even more nested, and in more confusing ways", preprocessor.PreprocessText(str3));
    }
}
