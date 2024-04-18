# AnimationCurveUtility
<mark style="color: lightgray; background-color: #191a18">AnimationCurveUtility</mark> is a static utility class that allows easy creation of various [AnimationCurves](https://docs.unity3d.com/ScriptReference/AnimationCurve.html).
The full API docs can be found [here](../api/TMPEffects.Extensions.AnimationCurveUtility.yml).

<style>
.center {
  display: block;
  margin-left: auto;
  margin-right: auto;
  margin-top: 2em;
  margin-bottom: 2em;
  width: 40%;
}
</style>

## Predefined curves
All of the easing functions presented at this link are implemented: https://easings.net/  
You can create the corresponding AnimationCurve like this: <mark style="color: lightgray; background-color: #191a18">AnimationCurveUtility.EaseInOutSine()</mark>.  
You can also get the Bézier points (don't worry about what this means if you don't know :smile:) that define the AnimationCurve, so you can manipulate them to easily
create slightly modified versions of the existing curves using the Bézier constructors.

## Bezier constructors
You can create AnimationCurves using Bézier points.  
Simply call <mark style="color: lightgray; background-color: #191a18">Bezier(params Vector2[] points)</mark> with your points Bézier points.  
The method will automatically infer whether you are creating a linear, quadratic or cubic Bézier curve based on the amount of points.  
If the amount of points does not clearly indicate one specific type, higher degree Bézier curves are preferred.  
There are also the <mark style="color: lightgray; background-color: #191a18">LinearBezier</mark>, <mark style="color: lightgray; background-color: #191a18">QuadraticBezier</mark>, and <mark style="color: lightgray; background-color: #191a18">CubicBezier</mark> methods, if you want to make sure the correct degree Bézier curve is created.

:warning: When creating your own AnimationCurves like this, always keep in the back of your mind that Unity's AnimationCurves use time as input; this means the Bézier curve must at all times advance on the X axis, or you will get an invalid AnimationCurve.  
For example, imagine the quadratic curve defined by the points (0,0), (0,1), (1,1):

<img src = "../images/000111.svg" alt="Quadratic Bezier curve" class="center"/>

This will yield an invalid curve! Consider the very beginning of this curve. At the very beginning, the curve moves perfectly straight up; that is not possible in AnimationCurves.  
Something even more extreme, like the curve moving "back" / to the left, is of course not possible either:

<img src = "../images/000111butworse.svg" alt="Quadratic Bezier curve" class="center"/>

---
Huge props to qwe321qwe321qwe321 on [GitHub](https://github.com/qwe321qwe321qwe321/Unity-EasingAnimationCurve) for his <mark style="color: lightgray; background-color: #191a18">BezierToAnimationCurve</mark> implementation, as well as the optimized curve points!
