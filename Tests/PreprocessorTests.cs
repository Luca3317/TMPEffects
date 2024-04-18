//using System.Collections;
//using System.Collections.Generic;
//using NUnit.Framework;
//using UnityEngine;
//using UnityEngine.TestTools;
//using Unity.PerformanceTesting;
//using TMPEffects.Databases;

//public class PreprocessorTests
//{
//    const string stress = "<test>Hello there</test><test>Hello there</test><test>Hello there</test><test>Hello there</test><test>Hello there</test><test>Hello there</test><test>Hello there</test><test>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><test><fake>Hello there</test>" +
//        "<test>Hello<fake> there</test><test>Hello there</test><test>Hello there</test><test>Hello<fake> there</test><test>Hello there</test>";

//    const string stressParsed = "Hello thereHello thereHello thereHello thereHello thereHello thereHello thereHello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there<fake>Hello thereHello thereHello there<fake>Hello there" +
//        "Hello<fake> thereHello thereHello thereHello<fake> thereHello there";

//    const string simple = "<test><fake>Hello there</test><test><fake>Hello there</test><test>Hello there</test><test>Hello there</test><fake><test>Hello there</test><test>Hello <fake>there</test><test>Hello there</test><test>Hello <fake>there</test>";
//    const string simpleParsed = "<fake>Hello there<fake>Hello thereHello thereHello there<fake>Hello thereHello <fake>thereHello thereHello <fake>there";


//    //[Test, Performance]
//    //public void OldStressTest()
//    //{
//    //    TMPEffectsDatabase db = (TMPEffectsDatabase)Resources.Load("DefaultDatabase");
//    //    OLDTMPEffectPreProcessor preProcessor = new OLDTMPEffectPreProcessor(db);

//    //    Measure.Method(() => preProcessor.PreprocessText(stress)).WarmupCount(5).MeasurementCount(1000).GC().Run();
//    //}

//    //[Test, Performance]
//    //public void NewStressTest()
//    //{
//    //    TMPEffectsDatabase db = (TMPEffectsDatabase)Resources.Load("DefaultDatabase");
//    //    TMPEffectPreProcessor preProcessor = new TMPEffectPreProcessor(db);

//    //    Measure.Method(() => preProcessor.PreprocessText(stress)).WarmupCount(5).MeasurementCount(1000).GC().Run();
//    //}

//    //[Test, Performance]
//    //public void OldSimpleTest()
//    //{
//    //    TMPEffectsDatabase db = (TMPEffectsDatabase)Resources.Load("DefaultDatabase");
//    //    OLDTMPEffectPreProcessor preProcessor = new OLDTMPEffectPreProcessor(db);

//    //    Measure.Method(() => preProcessor.PreprocessText(simple)).WarmupCount(5).MeasurementCount(1000).GC().Run();
//    //}

//    //[Test, Performance]
//    //public void NewSimpleTest()
//    //{
//    //    TMPEffectsDatabase db = (TMPEffectsDatabase)Resources.Load("DefaultDatabase");
//    //    TMPEffectPreProcessor preProcessor = new TMPEffectPreProcessor(db);

//    //    Measure.Method(() => preProcessor.PreprocessText(simple)).WarmupCount(5).MeasurementCount(1000).GC().Run();
//    //}

//    //[Test, Performance]
//    //public void Switchable()
//    //{
//    //    TMPEffectsDatabase db = (TMPEffectsDatabase)Resources.Load("DefaultDatabase");
//    //    TMPCommandDatabase db2 = (TMPCommandDatabase)Resources.Load("DefaultCommandDatabase");
//    //    AnimationTagPreprocessor atp = new(db);
//    //    CommandTagPreprocessor ctp = new(db2);
//    //    EventTagPreprocessor etp = new();

//    //    TMPEffectPreProcessor sw = new();
//    //    sw.RegisterPreprocessor(ParsingUtility.NO_PREFIX, atp);
//    //    sw.RegisterPreprocessor('!', ctp);
//    //    sw.RegisterPreprocessor('#', etp);

//    //    Measure.Method(() => sw.PreprocessText(stress)).WarmupCount(5).MeasurementCount(1000).GC().Run();
//    //}

//    //[Test, Performance]
//    //public void Normal()
//    //{
//    //    TMPEffectsDatabase db = (TMPEffectsDatabase)Resources.Load("DefaultDatabase");
//    //    TMPCommandDatabase db2 = (TMPCommandDatabase)Resources.Load("DefaultCommandDatabase");
//    //    TMPEffectPreProcessor normal = new TMPEffectPreProcessor(db2);

//    //    Measure.Method(() => normal.PreprocessText(stress)).WarmupCount(5).MeasurementCount(1000).GC().Run();
//    //}

//    [Test, Performance]
//    public void StressSubstring()
//    {
//        TMPEffectsDatabase db = (TMPEffectsDatabase)Resources.Load("DefaultDatabase");
//        TMPCommandDatabase db2 = (TMPCommandDatabase)Resources.Load("DefaultCommandDatabase");
//        AnimationTagProcessor atp = new(db);
//        CommandTagProcessor ctp = new(db2);
//        EventTagProcessor etp = new();

//        TMPTextProcessor processor = new();
//        processor.RegisterProcessor(ParsingUtility.NO_PREFIX, atp);
//        processor.RegisterProcessor('#', etp);
//        processor.RegisterProcessor('!', ctp);

//        Measure.Method(() => { processor.PreprocessText(stress); processor.ProcessTags(stress, stressParsed); }).WarmupCount(5).MeasurementCount(1000).GC().Run();
//    }

//    [Test, Performance]
//    public void SimpleSubstring()
//    {
//        TMPEffectsDatabase db = (TMPEffectsDatabase)Resources.Load("DefaultDatabase");
//        TMPCommandDatabase db2 = (TMPCommandDatabase)Resources.Load("DefaultCommandDatabase");
//        AnimationTagProcessor atp = new(db);
//        CommandTagProcessor ctp = new(db2);
//        EventTagProcessor etp = new();

//        TMPTextProcessor processor = new();
//        processor.RegisterProcessor(ParsingUtility.NO_PREFIX, atp);
//        processor.RegisterProcessor('#', etp);
//        processor.RegisterProcessor('!', ctp);

//        Measure.Method(() => { processor.PreprocessText(simple); processor.ProcessTags(simple, simpleParsed); }).WarmupCount(5).MeasurementCount(1000).GC().Run();
//    }
//}
