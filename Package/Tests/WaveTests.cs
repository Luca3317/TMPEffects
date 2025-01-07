using NUnit.Framework;
using TMPEffects.Extensions;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEngine;

public class WaveTests
{
    [Test]
    public void CreationTests()
    {
        Wave wave = new Wave();
        Assert.AreEqual(1, wave.Amplitude);
        Assert.AreEqual(1, wave.UpPeriod);
        Assert.AreEqual(1, wave.DownPeriod);
        Assert.AreEqual(1, wave.EffectiveUpPeriod);
        Assert.AreEqual(1, wave.EffectiveDownPeriod);
        Assert.AreEqual(2, wave.Period);
        Assert.AreEqual(2, wave.EffectivePeriod);
        Assert.AreEqual(0.5f, wave.Frequency);
        Assert.AreEqual(0, wave.TroughWait);
        Assert.AreEqual(0, wave.CrestWait);
        Assert.AreEqual(AnimationCurveUtility.EaseInOutSine(), wave.DownwardCurve);
        Assert.AreEqual(AnimationCurveUtility.EaseInOutSine(), wave.UpwardCurve);

        wave = new Wave(AnimationCurveUtility.EaseInBack(), AnimationCurveUtility.EaseOutBack(), 2f, 3f, 10f);
        Assert.AreEqual(10, wave.Amplitude);
        Assert.AreEqual(2, wave.UpPeriod);
        Assert.AreEqual(3, wave.DownPeriod);
        Assert.AreEqual(2, wave.EffectiveUpPeriod);
        Assert.AreEqual(3, wave.EffectiveDownPeriod);
        Assert.AreEqual(5, wave.Period);
        Assert.AreEqual(5, wave.EffectivePeriod);
        Assert.AreEqual(0.2f, wave.Frequency);
        Assert.AreEqual(0, wave.TroughWait);
        Assert.AreEqual(0, wave.CrestWait);
        Assert.AreEqual(AnimationCurveUtility.EaseInBack(), wave.UpwardCurve);
        Assert.AreEqual(AnimationCurveUtility.EaseOutBack(), wave.DownwardCurve);

        wave = new Wave(AnimationCurveUtility.EaseInBack(), AnimationCurveUtility.EaseOutBack(), 2f, 3f, 10f, 10f,
            12f);
        Assert.AreEqual(10, wave.Amplitude);
        Assert.AreEqual(2, wave.UpPeriod);
        Assert.AreEqual(3, wave.DownPeriod);
        Assert.AreEqual(2, wave.EffectiveUpPeriod);
        Assert.AreEqual(3, wave.EffectiveDownPeriod);
        Assert.AreEqual(5, wave.Period);
        Assert.AreEqual(5, wave.EffectivePeriod);
        Assert.AreEqual(0.2f, wave.Frequency);
        Assert.AreEqual(10, wave.CrestWait);
        Assert.AreEqual(12, wave.TroughWait);
        Assert.AreEqual(AnimationCurveUtility.EaseInBack(), wave.UpwardCurve);
        Assert.AreEqual(AnimationCurveUtility.EaseOutBack(), wave.DownwardCurve);
    }
    
    [Test]
    public void EvaluateAsWaveTests()
    {
        Wave wave = new Wave();

        var eval = wave.EvaluateAsWave(0, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave(wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(1, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave(wave.EffectivePeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave(wave.EffectivePeriod * 1234, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave(wave.EffectiveUpPeriod / 2f, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(1, eval.Direction);
        
        eval = wave.EvaluateAsWave((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        eval = wave.EvaluateAsWave((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 1234),
            0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        // ~277 hours. Much higher and floating point precision messes it up.
        eval = wave.EvaluateAsWave(
            (wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 500000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);


        wave = new Wave(AnimationCurveUtility.Linear(), AnimationCurveUtility.Linear(), 2f, 3f, 10f);

        eval = wave.EvaluateAsWave(0, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave(wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(10, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave(wave.EffectivePeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave(wave.EffectivePeriod * 1234, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave(wave.EffectiveUpPeriod / 2f, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        // ~277 hours. Much higher and floating point precision messes it up.
        eval = wave.EvaluateAsWave(
            (wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 500000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);


        wave = new Wave(AnimationCurveUtility.Linear(), AnimationCurveUtility.Linear(), 2f, 3f, 10f, 10f, 12f);

        eval = wave.EvaluateAsWave(0, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave(wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(10, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave(wave.EffectivePeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave(wave.EffectivePeriod * 1234, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave(wave.EffectiveUpPeriod / 2f, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsWave((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        // ~277 hours. Much higher and floating point precision messes it up.
        eval = wave.EvaluateAsWave(
            (wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 500000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);
    }

    [Test]
    public void EvaluateAsPulseTests()
    {
        Wave wave = new Wave();

        var eval = wave.EvaluateAsPulse(0, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse(wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(1, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse(wave.EffectivePeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse(wave.EffectivePeriod * 1234, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse(wave.EffectiveUpPeriod / 2f, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        eval = wave.EvaluateAsPulse((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 1234),
            0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        // ~277 hours. Much higher and floating point precision messes it up.
        eval = wave.EvaluateAsPulse(
            (wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 500000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);


        wave = new Wave(AnimationCurveUtility.Linear(), AnimationCurveUtility.Linear(), 2f, 3f, 10f);

        eval = wave.EvaluateAsPulse(0, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse(wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(10, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse(wave.EffectivePeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse(wave.EffectivePeriod * 1234, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse(wave.EffectiveUpPeriod / 2f, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        // ~277 hours. Much higher and floating point precision messes it up.
        eval = wave.EvaluateAsPulse(
            (wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 500000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);


        wave = new Wave(AnimationCurveUtility.Linear(), AnimationCurveUtility.Linear(), 2f, 3f, 10f, 10f, 12f);

        eval = wave.EvaluateAsPulse(0, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse(wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(10, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse(wave.EffectivePeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        eval = wave.EvaluateAsPulse(wave.EffectivePeriod + wave.TroughWait, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);
        
        eval = wave.EvaluateAsPulse((wave.EffectivePeriod + wave.TroughWait) * 1234, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse(wave.EffectiveUpPeriod / 2f, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsPulse((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        eval = wave.EvaluateAsPulse(
            (wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + ((wave.EffectivePeriod + wave.TroughWait) * 50000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);
    }
    
    [Test]
    public void EvaluateAsInvertedPulseTests()
    {
        Wave wave = new Wave();

        var eval = wave.EvaluateAsInvertedPulse(0, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(1, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(wave.EffectivePeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(wave.EffectivePeriod * 1234, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(wave.EffectiveUpPeriod / 2f, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 1234),
            0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        // ~277 hours. Much higher and floating point precision messes it up.
        eval = wave.EvaluateAsInvertedPulse(
            (wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 500000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);


        wave = new Wave(AnimationCurveUtility.Linear(), AnimationCurveUtility.Linear(), 2f, 3f, 10f);

        eval = wave.EvaluateAsInvertedPulse(0, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(10, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(wave.EffectivePeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(wave.EffectivePeriod * 1234, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(wave.EffectiveUpPeriod / 2f, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        // ~277 hours. Much higher and floating point precision messes it up.
        eval = wave.EvaluateAsInvertedPulse(
            (wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 500000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);


        wave = new Wave(AnimationCurveUtility.Linear(), AnimationCurveUtility.Linear(), 2f, 3f, 10f, 10f, 12f);

        eval = wave.EvaluateAsInvertedPulse(0, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(10, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(wave.EffectiveUpPeriod + wave.CrestWait, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(10, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(wave.EffectivePeriod + wave.CrestWait, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);
        
        eval = wave.EvaluateAsInvertedPulse((wave.EffectivePeriod + wave.CrestWait) * 1234, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(wave.EffectiveUpPeriod / 2f, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + wave.CrestWait, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        eval = wave.EvaluateAsInvertedPulse(
            (wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + wave.CrestWait + ((wave.EffectivePeriod + wave.CrestWait) * 50000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);
    }
    
    [Test]
    public void EvaluateAsOneDirectionalPulseTests()
    {
        Wave wave = new Wave();

        var eval = wave.EvaluateAsOneDirectionalPulse(0, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(1, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectivePeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectivePeriod * 1234, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectiveUpPeriod / 2f, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 1234),
            0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        // ~277 hours. Much higher and floating point precision messes it up.
        eval = wave.EvaluateAsOneDirectionalPulse(
            (wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 500000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);


        wave = new Wave(AnimationCurveUtility.Linear(), AnimationCurveUtility.Linear(), 2f, 3f, 10f);

        eval = wave.EvaluateAsOneDirectionalPulse(0, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(10, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectivePeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectivePeriod * 1234, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectiveUpPeriod / 2f, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        // ~277 hours. Much higher and floating point precision messes it up.
        eval = wave.EvaluateAsOneDirectionalPulse(
            (wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + (wave.EffectivePeriod * 500000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);


        wave = new Wave(AnimationCurveUtility.Linear(), AnimationCurveUtility.Linear(), 2f, 3f, 10f, 10f, 12f);

        eval = wave.EvaluateAsOneDirectionalPulse(0, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectiveUpPeriod, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(10, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectiveUpPeriod + wave.CrestWait, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(10, eval.Value);
        Assert.AreEqual(1, eval.Direction);
        
        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectivePeriod + wave.CrestWait, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectivePeriod + wave.CrestWait + wave.TroughWait, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);
        
        eval = wave.EvaluateAsOneDirectionalPulse((wave.EffectivePeriod + wave.CrestWait + wave.TroughWait) * 1234, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(wave.EffectiveUpPeriod / 2f, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse((wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + wave.CrestWait, 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);

        eval = wave.EvaluateAsOneDirectionalPulse(
            (wave.EffectiveDownPeriod / 2f) + wave.EffectiveUpPeriod + wave.CrestWait + ((wave.EffectivePeriod + wave.CrestWait + wave.TroughWait) * 50000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(wave.Amplitude / 2f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);
        
        eval = wave.EvaluateAsOneDirectionalPulse(
            wave.EffectiveDownPeriod + wave.EffectiveUpPeriod + wave.CrestWait + (wave.TroughWait / 2f) + ((wave.EffectivePeriod + wave.CrestWait + wave.TroughWait) * 50000), 0);
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(0f, eval.Value);
        Assert.AreEqual(-1, eval.Direction);
    }
}