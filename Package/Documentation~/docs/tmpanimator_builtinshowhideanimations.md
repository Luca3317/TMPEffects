# Built-in animations

This section gives you a complete overview of all built-in show and hide animations and their parameters (for basic / scene animations see the respective sections).
Each of the animations listed here has both a show and a hide version.

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
            <mark style="color: lightgray; background-color: #191a18">duration</mark>, <mark style="color: lightgray; background-color: #191a18">dur</mark>, <mark style="color: lightgray; background-color: #191a18">d</mark></li>
            <li> Curve: The curve used for fading in / out.<br>
            <mark style="color: lightgray; background-color: #191a18">curve</mark>, <mark style="color: lightgray; background-color: #191a18">crv</mark>, <mark style="color: lightgray; background-color: #191a18">c</mark></li>
            <li> Anchor: The anchor that is faded in / out from.<br>
            <mark style="color: lightgray; background-color: #191a18">anchor</mark>, <mark style="color: lightgray; background-color: #191a18">anc</mark>, <mark style="color: lightgray; background-color: #191a18">a</mark></li>
            <li> Direction: The direction that is faded in / out from.<br>
            <mark style="color: lightgray; background-color: #191a18">direction</mark>, <mark style="color: lightgray; background-color: #191a18">dir</mark></li>
            <li> <mark style="color: lightgray; background-color: #191a18">Show:</mark>StartOpacity: The opacity that is faded in from.<br>
            <mark style="color: lightgray; background-color: #191a18">startopacity</mark>, <mark style="color: lightgray; background-color: #191a18">startop</mark>, <mark style="color: lightgray; background-color: #191a18">start</mark></li>
            <li> <mark style="color: lightgray; background-color: #191a18">Hide:</mark>TargetOpacity: The opacity that is faded out from.<br>
            <mark style="color: lightgray; background-color: #191a18">targetopacity</mark>, <mark style="color: lightgray; background-color: #191a18">targetop</mark>, <mark style="color: lightgray; background-color: #191a18">target</mark></li>
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
            <mark style="color: lightgray; background-color: #191a18">duration</mark>, <mark style="color: lightgray; background-color: #191a18">dur</mark>, <mark style="color: lightgray; background-color: #191a18">d</mark></li>
            <li> Curve: The curve used for getting the t-value to interpolate between the angles.<br>
            <mark style="color: lightgray; background-color: #191a18">curve</mark>, <mark style="color: lightgray; background-color: #191a18">crv</mark>, <mark style="color: lightgray; background-color: #191a18">c</mark></li>
            <li> Pivot: The pivot position of the rotation.<br>
            <mark style="color: lightgray; background-color: #191a18">pivot</mark>, <mark style="color: lightgray; background-color: #191a18">pv</mark>, <mark style="color: lightgray; background-color: #191a18">p</mark></li>
            <li> StartAngle: The starting euler angles.<br>
            <mark style="color: lightgray; background-color: #191a18">startangle</mark>, <mark style="color: lightgray; background-color: #191a18">start</mark></li>
            <li> TargetAngle: The target euler angles.<br>
            <mark style="color: lightgray; background-color: #191a18">targetangle</mark>, <mark style="color: lightgray; background-color: #191a18">target</mark></li>
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
            <mark style="color: lightgray; background-color: #191a18">duration</mark>, <mark style="color: lightgray; background-color: #191a18">dur</mark>, <mark style="color: lightgray; background-color: #191a18">d</mark></li>
            <li> Curve: The curve used for getting the t-value to interpolate between the scales.<br>
            <mark style="color: lightgray; background-color: #191a18">curve</mark>, <mark style="color: lightgray; background-color: #191a18">crv</mark>, <mark style="color: lightgray; background-color: #191a18">c</mark></li>
            <li> <mark style="color: lightgray; background-color: #191a18">Show</mark> StartScale: The scale to start growing to the initial scale from.<br>
            <mark style="color: lightgray; background-color: #191a18">startscale</mark>, <mark style="color: lightgray; background-color: #191a18">startscl</mark>, <mark style="color: lightgray; background-color: #191a18">start</mark></li>
            <li> <mark style="color: lightgray; background-color: #191a18">Hide</mark> TargetScale: The scale to grow to from the initial scale.<br>
            <mark style="color: lightgray; background-color: #191a18">targetscale</mark>, <mark style="color: lightgray; background-color: #191a18">targetscl</mark>, <mark style="color: lightgray; background-color: #191a18">target</mark></li>
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
            <mark style="color: lightgray; background-color: #191a18">duration</mark>, <mark style="color: lightgray; background-color: #191a18">dur</mark>, <mark style="color: lightgray; background-color: #191a18">d</mark></li>
            <li> Curve: The curve used for getting the t-value to interpolate between the percentages.<br>
            <mark style="color: lightgray; background-color: #191a18">curve</mark>, <mark style="color: lightgray; background-color: #191a18">crv</mark>, <mark style="color: lightgray; background-color: #191a18">c</mark></li>
            <li> Anchor: The anchor from where the character spreads.<br>
            <mark style="color: lightgray; background-color: #191a18">anchor</mark>, <mark style="color: lightgray; background-color: #191a18">anc</mark>, <mark style="color: lightgray; background-color: #191a18">a</mark></li>
            <li> Direction: The direction in which the character spreads.<br>
            <mark style="color: lightgray; background-color: #191a18">direction</mark>, <mark style="color: lightgray; background-color: #191a18">dir</mark>, <mark style="color: lightgray; background-color: #191a18">d</mark></li>
            <li> StartPercentage: The start percentage of the spread, 0 being fully hidden.<br>
            <mark style="color: lightgray; background-color: #191a18">startpercentage</mark>, <mark style="color: lightgray; background-color: #191a18">start</mark></li>
            <li> TargetPercentage: The target percentage of the spread, 1 being fully shown.<br>
            <mark style="color: lightgray; background-color: #191a18">targetpercentage</mark>, <mark style="color: lightgray; background-color: #191a18">target</mark></li>
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
            <mark style="color: lightgray; background-color: #191a18">duration</mark>, <mark style="color: lightgray; background-color: #191a18">dur</mark>, <mark style="color: lightgray; background-color: #191a18">d</mark></li>
            <li> Curve: The curve used for getting the t-value to interpolate between the start and target position.<br>
            <mark style="color: lightgray; background-color: #191a18">curve</mark>, <mark style="color: lightgray; background-color: #191a18">crv</mark>, <mark style="color: lightgray; background-color: #191a18">c</mark></li>
            <li> <mark style="color: lightgray; background-color: #191a18">Show</mark> StartPositon: The postion to move the character in from.<br>
            <mark style="color: lightgray; background-color: #191a18">startposition</mark>, <mark style="color: lightgray; background-color: #191a18">startpos</mark>, <mark style="color: lightgray; background-color: #191a18">start</mark></li>
            <li> <mark style="color: lightgray; background-color: #191a18">Hide</mark> TargetPosition: The postion to move the character to.<br>
            <mark style="color: lightgray; background-color: #191a18">targetposition</mark>, <mark style="color: lightgray; background-color: #191a18">targetpos</mark>, <mark style="color: lightgray; background-color: #191a18">target</mark></li>
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
            <mark style="color: lightgray; background-color: #191a18">duration</mark>, <mark style="color: lightgray; background-color: #191a18">dur</mark>, <mark style="color: lightgray; background-color: #191a18">d</mark></li>
            <li> MaxXAmplitude: The maximum X amplitude of each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">maxxamplitude</mark>, <mark style="color: lightgray; background-color: #191a18">maxxamp</mark>, <mark style="color: lightgray; background-color: #191a18">maxxa</mark>, <mark style="color: lightgray; background-color: #191a18">maxx</mark></li>
            <li> MinXAmplitude: The minimum X amplitude of each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">minxamplitude</mark>, <mark style="color: lightgray; background-color: #191a18">minxamp</mark>, <mark style="color: lightgray; background-color: #191a18">minxa</mark>, <mark style="color: lightgray; background-color: #191a18">minx</mark></li>
            <li> MaxYAmplitude: The maximum Y amplitude of each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">maxyamplitude</mark>, <mark style="color: lightgray; background-color: #191a18">maxyamp</mark>, <mark style="color: lightgray; background-color: #191a18">maxya</mark>, <mark style="color: lightgray; background-color: #191a18">maxy</mark></li>
            <li> MinYAmplitude: The minimum Y amplitude of each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">minyamplitude</mark>, <mark style="color: lightgray; background-color: #191a18">minyamp</mark>, <mark style="color: lightgray; background-color: #191a18">minya</mark>, <mark style="color: lightgray; background-color: #191a18">miny</mark></li>
            <li> MaxWait: The minimum amount of time to wait after each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">maxwait</mark>, <mark style="color: lightgray; background-color: #191a18">maxw</mark></li>
            <li> MinWait: The maximum amount of time to wait after each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">minwait</mark>, <mark style="color: lightgray; background-color: #191a18">minw</mark></li>
            <li> WaitCurve: The curve that defines the falloff of the wait between each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">waitcurve</mark>, <mark style="color: lightgray; background-color: #191a18">waitcrv</mark>, <mark style="color: lightgray; background-color: #191a18">waitc</mark></li>
            <li> AmplitudeCurve: The curve that defines the falloff of the amplitude of each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">amplitudecurve</mark>, <mark style="color: lightgray; background-color: #191a18">amplitudecrv</mark>, <mark style="color: lightgray; background-color: #191a18">amplitudec</mark>, <mark style="color: lightgray; background-color: #191a18">ampcurve</mark>, <mark style="color: lightgray; background-color: #191a18">ampcrv</mark>, <mark style="color: lightgray; background-color: #191a18">ampc</mark></li>
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
            <mark style="color: lightgray; background-color: #191a18">duration</mark>, <mark style="color: lightgray; background-color: #191a18">dur</mark>, <mark style="color: lightgray; background-color: #191a18">d</mark></li>
            <li> Characters: The pool of characters to change to.<br>
            <mark style="color: lightgray; background-color: #191a18">characters</mark>, <mark style="color: lightgray; background-color: #191a18">chars</mark>, <mark style="color: lightgray; background-color: #191a18">char</mark>, <mark style="color: lightgray; background-color: #191a18">c</mark></li>
            <li> Probability: The probability to change to a character different from the original.<br>
            <mark style="color: lightgray; background-color: #191a18">probability</mark>, <mark style="color: lightgray; background-color: #191a18">prob</mark>, <mark style="color: lightgray; background-color: #191a18">p</mark></li>
            <li> MinWait: The minimum amount of time to wait once a character changed (or did not change).<br>
            <mark style="color: lightgray; background-color: #191a18">minwait</mark>, <mark style="color: lightgray; background-color: #191a18">minw</mark>, <mark style="color: lightgray; background-color: #191a18">min</mark>
            <li> MaxWait: The maximum amount of time to wait once a character changed (or did not change).<br>
            <mark style="color: lightgray; background-color: #191a18">maxwait</mark>, <mark style="color: lightgray; background-color: #191a18">maxw</mark>, <mark style="color: lightgray; background-color: #191a18">max</mark>
            <li> AutoCase: Whether to ensure capitalized characters are only changed to other capitalized characters, and vice versa.<br>
            <mark style="color: lightgray; background-color: #191a18">autocase</mark>, <mark style="color: lightgray; background-color: #191a18">case</mark>
            <li> WaitCurve: The curve that defines the falloff of the wait between each change.<br>
            <mark style="color: lightgray; background-color: #191a18">waitcurve</mark>, <mark style="color: lightgray; background-color: #191a18">waitcrv</mark>, <mark style="color: lightgray; background-color: #191a18">waitc</mark></li>
            <li> Probability: The curve that defines the falloff of the probability of changing to a character other than the original.<br>
            <mark style="color: lightgray; background-color: #191a18">probabilitycurve</mark>, <mark style="color: lightgray; background-color: #191a18">probabilitycrv</mark>, <mark style="color: lightgray; background-color: #191a18">probabilityc</mark>, <mark style="color: lightgray; background-color: #191a18">probcurve</mark>, <mark style="color: lightgray; background-color: #191a18">probcrv</mark>, <mark style="color: lightgray; background-color: #191a18">probc</mark></li>
        </ul>
    </div>
  </div>
</div>
</div>
</div>

<div></div>
<div></div>
</div>
