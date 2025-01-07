<link rel="stylesheet" type="text/css" href="../styles.css">

# Built-in show / hide animations

This section gives you a complete overview of all built-in show and hide animations and their parameters (for basic / scene animations see the respective sections).
Each of the animations listed here has both a show and a hide version.  
Show / hide animations are applied in the same manner as basic animations, except show animations use a '+' prefix: <mark class="markstyle">&lt;+shake&gt;TMPEffects&lt;/+shake&gt;</mark>
and hide animations use a '-' prefix: <mark class="markstyle">&lt;-fade&gt;TMPEffects&lt;/-&gt;</mark>.

<style>
.centered-video {

}

.anim-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
}

.grid-item {
  border: 1px solid black;
  padding: 10px;
}

.flex-row{
    display: flex;
    flex-direction: row;
    justify-content: space-evenly;
}

.flex-row > div {
    flex: 1;
}

.anim-container {
    margin-left: 1.2rem;
    margin-right: 1.2rem;
}

.wrap-collabsible {
  margin-bottom: 1.2rem 0;
  display: flex;
  justify-content: center;
  flex-direction: column;
  align-items: center;
}

input[type='checkbox'] {
  display: none;
}

.lbl-toggle {
  display: block;

  font-weight: bold;
  font-family: monospace;
  font-size: 1.2rem;
  text-transform: uppercase;
  text-align: center;

  padding: 1rem;

  color: #A77B0E;
  background: #FAE042;

  cursor: pointer;

  border-radius: 0px;
  transition: all 0.00s ease-out;

  user-select: none;
  width: 100%;
}

.lbl-toggle:hover {
  color: #7C5A0B;
}

.lbl-toggle::before {
  content: ' ';
  display: inline-block;

  border-top: 5px solid transparent;
  border-bottom: 5px solid transparent;
  border-left: 5px solid currentColor;
  vertical-align: middle;
  margin-right: .7rem;
  transform: translateY(-2px);

  transition: transform .0s ease-out;
}

.toggle:checked + .lbl-toggle::before {
  transform: rotate(90deg) translateX(-3px);
}

.collapsible-content {
  max-height: 0px;
  overflow: hidden;
  transition: max-height .00s ease-in-out;
}

.toggle:checked + .lbl-toggle + .collapsible-content {
  max-height: 100vh;
}

.toggle:checked + .lbl-toggle {
  border-bottom-right-radius: 0;
  border-bottom-left-radius: 0;
}

.collapsible-content .content-inner {
  background: rgba(250, 224, 66, .2);
  border-bottom: 1px solid rgba(250, 224, 66, .45);
  border-bottom-left-radius: 7px;
  border-bottom-right-radius: 7px;
  padding: .5rem 1rem;
}
</style>


<div class="anim-grid">

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/showhide/fade.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblefade" class="toggle" type="checkbox">
  <label for="collapsiblefade" class="lbl-toggle">fade Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
        <ul>
            <li> Duration: How long the animation will take to fully show / hide the character.<br>
            <mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></li>
            <li> Curve: The curve used for fading in / out.<br>
            <mark class="markstyle">curve</mark>, <mark class="markstyle">crv</mark>, <mark class="markstyle">c</mark></li>
            <li> Anchor: The anchor that is faded in / out from.<br>
            <mark class="markstyle">anchor</mark>, <mark class="markstyle">anc</mark>, <mark class="markstyle">a</mark></li>
            <li> Direction: The direction that is faded in / out from.<br>
            <mark class="markstyle">direction</mark>, <mark class="markstyle">dir</mark></li>
            <li> <mark class="markstyle">Show:</mark>StartOpacity: The opacity that is faded in from.<br>
            <mark class="markstyle">startopacity</mark>, <mark class="markstyle">startop</mark>, <mark class="markstyle">start</mark></li>
            <li> <mark class="markstyle">Hide:</mark>TargetOpacity: The opacity that is faded out from.<br>
            <mark class="markstyle">targetopacity</mark>, <mark class="markstyle">targetop</mark>, <mark class="markstyle">target</mark></li>
        </ul>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/showhide/pivot.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblepivot" class="toggle" type="checkbox">
  <label for="collapsiblepivot" class="lbl-toggle">pivot Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
            <li> Duration: How long the animation will take to fully show / hide the character.<br>
            <mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></li>
            <li> Curve: The curve used for getting the t-value to interpolate between the angles.<br>
            <mark class="markstyle">curve</mark>, <mark class="markstyle">crv</mark>, <mark class="markstyle">c</mark></li>
            <li> Pivot: The pivot position of the rotation.<br>
            <mark class="markstyle">pivot</mark>, <mark class="markstyle">pv</mark>, <mark class="markstyle">p</mark></li>
            <li> StartAngle: The starting euler angles.<br>
            <mark class="markstyle">startangle</mark>, <mark class="markstyle">start</mark></li>
            <li> TargetAngle: The target euler angles.<br>
            <mark class="markstyle">targetangle</mark>, <mark class="markstyle">target</mark></li>
        </ul>
      </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video class="centered-video" style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/showhide/grow.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>
  <input id="collapsiblegrow" class="toggle" type="checkbox">
  <label for="collapsiblegrow" class="lbl-toggle">grow Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
            <li> Duration: How long the animation will take to fully show / hide the character.<br>
            <mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></li>
            <li> Curve: The curve used for getting the t-value to interpolate between the scales.<br>
            <mark class="markstyle">curve</mark>, <mark class="markstyle">crv</mark>, <mark class="markstyle">c</mark></li>
            <li> <mark class="markstyle">Show</mark> StartScale: The scale to start growing to the initial scale from.<br>
            <mark class="markstyle">startscale</mark>, <mark class="markstyle">startscl</mark>, <mark class="markstyle">start</mark></li>
            <li> <mark class="markstyle">Hide</mark> TargetScale: The scale to grow to from the initial scale.<br>
            <mark class="markstyle">targetscale</mark>, <mark class="markstyle">targetscl</mark>, <mark class="markstyle">target</mark></li>
        </ul>
      </p>
    </div>
  </div>
</div>
</div>


<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/showhide/spread.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblespread" class="toggle" type="checkbox">
  <label for="collapsiblespread" class="lbl-toggle">spread Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
        <ul>
            <li> Duration: How long the animation will take to fully show / hide the character.<br>
            <mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></li>
            <li> Curve: The curve used for getting the t-value to interpolate between the percentages.<br>
            <mark class="markstyle">curve</mark>, <mark class="markstyle">crv</mark>, <mark class="markstyle">c</mark></li>
            <li> Anchor: The anchor from where the character spreads.<br>
            <mark class="markstyle">anchor</mark>, <mark class="markstyle">anc</mark>, <mark class="markstyle">a</mark></li>
            <li> Direction: The direction in which the character spreads.<br>
            <mark class="markstyle">direction</mark>, <mark class="markstyle">dir</mark>, <mark class="markstyle">d</mark></li>
            <li> StartPercentage: The start percentage of the spread, 0 being fully hidden.<br>
            <mark class="markstyle">startpercentage</mark>, <mark class="markstyle">start</mark></li>
            <li> TargetPercentage: The target percentage of the spread, 1 being fully shown.<br>
            <mark class="markstyle">targetpercentage</mark>, <mark class="markstyle">target</mark></li>
        </ul>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/showhide/move.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblemove" class="toggle" type="checkbox">
  <label for="collapsiblemove" class="lbl-toggle">move Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
            <li> Duration: How long the animation will take to fully show / hide the character.<br>
            <mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></li>
            <li> Curve: The curve used for getting the t-value to interpolate between the start and target position.<br>
            <mark class="markstyle">curve</mark>, <mark class="markstyle">crv</mark>, <mark class="markstyle">c</mark></li>
            <li> <mark class="markstyle">Show</mark> StartPositon: The postion to move the character in from.<br>
            <mark class="markstyle">startposition</mark>, <mark class="markstyle">startpos</mark>, <mark class="markstyle">start</mark></li>
            <li> <mark class="markstyle">Hide</mark> TargetPosition: The postion to move the character to.<br>
            <mark class="markstyle">targetposition</mark>, <mark class="markstyle">targetpos</mark>, <mark class="markstyle">target</mark></li>
        </ul>
      </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/showhide/shake.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsibleshake" class="toggle" type="checkbox">
  <label for="collapsibleshake" class="lbl-toggle">shake Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
            <li> Duration: How long the animation will take to fully show / hide the character.<br>
            <mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></li>
            <li> MaxXAmplitude: The maximum X amplitude of each shake.<br>
            <mark class="markstyle">maxxamplitude</mark>, <mark class="markstyle">maxxamp</mark>, <mark class="markstyle">maxxa</mark>, <mark class="markstyle">maxx</mark></li>
            <li> MinXAmplitude: The minimum X amplitude of each shake.<br>
            <mark class="markstyle">minxamplitude</mark>, <mark class="markstyle">minxamp</mark>, <mark class="markstyle">minxa</mark>, <mark class="markstyle">minx</mark></li>
            <li> MaxYAmplitude: The maximum Y amplitude of each shake.<br>
            <mark class="markstyle">maxyamplitude</mark>, <mark class="markstyle">maxyamp</mark>, <mark class="markstyle">maxya</mark>, <mark class="markstyle">maxy</mark></li>
            <li> MinYAmplitude: The minimum Y amplitude of each shake.<br>
            <mark class="markstyle">minyamplitude</mark>, <mark class="markstyle">minyamp</mark>, <mark class="markstyle">minya</mark>, <mark class="markstyle">miny</mark></li>
            <li> MaxWait: The minimum amount of time to wait after each shake.<br>
            <mark class="markstyle">maxwait</mark>, <mark class="markstyle">maxw</mark></li>
            <li> MinWait: The maximum amount of time to wait after each shake.<br>
            <mark class="markstyle">minwait</mark>, <mark class="markstyle">minw</mark></li>
            <li> WaitCurve: The curve that defines the falloff of the wait between each shake.<br>
            <mark class="markstyle">waitcurve</mark>, <mark class="markstyle">waitcrv</mark>, <mark class="markstyle">waitc</mark></li>
            <li> AmplitudeCurve: The curve that defines the falloff of the amplitude of each shake.<br>
            <mark class="markstyle">amplitudecurve</mark>, <mark class="markstyle">amplitudecrv</mark>, <mark class="markstyle">amplitudec</mark>, <mark class="markstyle">ampcurve</mark>, <mark class="markstyle">ampcrv</mark>, <mark class="markstyle">ampc</mark></li>
        </ul>
      </p>
    </div>
  </div>
</div>
</div>


<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/showhide/char.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblechar" class="toggle" type="checkbox">
  <label for="collapsiblechar" class="lbl-toggle">char Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
        <ul>
            <li> Duration: How long the animation will take to fully show / hide the character.<br>
            <mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></li>
            <li> Characters: The pool of characters to change to.<br>
            <mark class="markstyle">characters</mark>, <mark class="markstyle">chars</mark>, <mark class="markstyle">char</mark>, <mark class="markstyle">c</mark></li>
            <li> Probability: The probability to change to a character different from the original.<br>
            <mark class="markstyle">probability</mark>, <mark class="markstyle">prob</mark>, <mark class="markstyle">p</mark></li>
            <li> MinWait: The minimum amount of time to wait once a character changed (or did not change).<br>
            <mark class="markstyle">minwait</mark>, <mark class="markstyle">minw</mark>, <mark class="markstyle">min</mark>
            <li> MaxWait: The maximum amount of time to wait once a character changed (or did not change).<br>
            <mark class="markstyle">maxwait</mark>, <mark class="markstyle">maxw</mark>, <mark class="markstyle">max</mark>
            <li> AutoCase: Whether to ensure capitalized characters are only changed to other capitalized characters, and vice versa.<br>
            <mark class="markstyle">autocase</mark>, <mark class="markstyle">case</mark>
            <li> WaitCurve: The curve that defines the falloff of the wait between each change.<br>
            <mark class="markstyle">waitcurve</mark>, <mark class="markstyle">waitcrv</mark>, <mark class="markstyle">waitc</mark></li>
            <li> Probability: The curve that defines the falloff of the probability of changing to a character other than the original.<br>
            <mark class="markstyle">probabilitycurve</mark>, <mark class="markstyle">probabilitycrv</mark>, <mark class="markstyle">probabilityc</mark>, <mark class="markstyle">probcurve</mark>, <mark class="markstyle">probcrv</mark>, <mark class="markstyle">probc</mark></li>
        </ul>
    </div>
  </div>
</div>
</div>
</div>

<div></div>
<div></div>
</div>
