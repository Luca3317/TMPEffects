<link rel="stylesheet" type="text/css" href="../styles.css">

# Built-in show / hide animations

This section gives you a complete overview of all built-in show and hide animations and their parameters (for basic / scene animations see the respective sections).
Each of the animations listed here has both a show and a hide version.  
Show / hide animations are applied in the same manner as basic animations, except show animations use a '+' prefix: <mark class="markstyle">&lt;+shake&gt;TMPEffects&lt;/+shake&gt;</mark>
and hide animations use a '-' prefix: <mark class="markstyle">&lt;-fade&gt;TMPEffects&lt;/-&gt;</mark>.  

Each parameter here specifies its type; you can find the overview of built-in types [here](parametertypes.md). In addition to this overview, the animation assets 
(located under Assets/TMPEffects/Resources, once you imported the required resources) all come with descriptive tooltips.

<style>

.switch {
  position: relative;
  display: inline-block;
  width: 30px;
  height: 17px;
  margin-left: 15px; 
  margin-right: 15px; 
}

/* Hide default HTML checkbox */
.switch input {
  opacity: 0;
  width: 0;
  height: 0;
}

/* The slider */
.slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #ccc;
  -webkit-transition: .4s;
  transition: .4s;
}

.slider:before {
  position: absolute;
  content: "";
  height: 13px;
  width: 13px;
  left: 2px;
  bottom: 2px;
  background-color: white;
  -webkit-transition: .4s;
  transition: .4s;
}

input:checked + .slider {
  background-color: #2196F3;
}

input:focus + .slider {
  box-shadow: 0 0 1px #2196F3;
}

input:checked + .slider:before {
  -webkit-transform: translateX(13px);
  -ms-transform: translateX(13px);
  transform: translateX(13px);
}

/* Rounded sliders */
.slider.round {
  border-radius: 17px;
}

.slider.round:before {
  border-radius: 50%;
}

#toggleButton { 
  padding: 10px 20px; 
  font-size: 16px; 
  cursor: pointer; 
} 

#content { 
  margin-top: 20px; 
  font-size: 18px; 
  display: none; 
}

.anim-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
}

.centered-video {

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
  min-width: 100%;
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
  min-width: 100%;
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
  min-width: 100%;
}

.toggles {
  display: flex;
  align-items: center;
  flex-direction: row;
}

.alias {
  display: none;
}

.descr {
  display: none;
}
</style>

<div class="toggles">
  Show Descriptions <label class="switch">
    <input type="checkbox">
    <span class="slider" onclick="toggleVisibilityDescr()"></span>
  </label> Show Aliases <label class="switch">
    <input type="checkbox">
    <span class="slider" onclick="toggleVisibilityAlias()"></span>
  </label>
</div>

<script>
  function toggleVisibilityDescr() {
    const elements = document.querySelectorAll('.descr');
    elements.forEach(element => {
      const style = window.getComputedStyle(element);
      if (style.display === 'none') {
        element.style.display = 'block';
      } else {
        element.style.display = 'none';
      }
    });
  }

  function toggleVisibilityAlias() {
    const elements = document.querySelectorAll('.alias');
    elements.forEach(element => {
      const style = window.getComputedStyle(element);
      if (style.display === 'none') {
        element.style.display = 'block';
      } else {
        element.style.display = 'none';
      }
    });
  }
</script>

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
            <li> Duration: <mark class="markstyle">float</mark>
            <div class="descr">How long the animation will take to fully show / hide the character.</div>
            <div class="alias"><mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></div></li>
            <li> Curve: <mark class="markstyle">AnimationCurve</mark>
            <div class="descr">The curve used for fading in / out.</div>
            <div class="alias"><mark class="markstyle">curve</mark>, <mark class="markstyle">crv</mark>, <mark class="markstyle">c</mark></div></li>
            <li> Anchor: <mark class="markstyle">Vector3</mark>
            <div class="descr">The anchor that is faded in / out from.</div>
            <div class="alias"><mark class="markstyle">anchor</mark>, <mark class="markstyle">anc</mark>, <mark class="markstyle">a</mark></div></li>
            <li> Direction: <mark class="markstyle">Vector3</mark>
            <div class="descr">The direction that is faded in / out from.</div>
            <div class="alias"><mark class="markstyle">direction</mark>, <mark class="markstyle">dir</mark></div></li>
            <li> <mark class="markstyle">Show:</mark>StartOpacity: <mark class="markstyle">float</mark>
            <div class="descr">The opacity that is faded in from.</div>
            <div class="alias"><mark class="markstyle">startopacity</mark>, <mark class="markstyle">startop</mark>, <mark class="markstyle">start</mark></div></li>
            <li> <mark class="markstyle">Hide:</mark>TargetOpacity: <mark class="markstyle">float</mark>
            <div class="descr">The opacity that is faded out from.</div>
            <div class="alias"><mark class="markstyle">targetopacity</mark>, <mark class="markstyle">targetop</mark>, <mark class="markstyle">target</mark></div></li>
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
            <li> Duration: <mark class="markstyle">float</mark>
            <div class="descr">How long the animation will take to fully show / hide the character.</div>
            <div class="alias"><mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></div></li>
            <li> Curve: <mark class="markstyle">AnimationCurve</mark>
            <div class="descr">The curve used for getting the t-value to interpolate between the angles.</div>
            <div class="alias"><mark class="markstyle">curve</mark>, <mark class="markstyle">crv</mark>, <mark class="markstyle">c</mark></div></li>
            <li> Pivot: <mark class="markstyle">TypedVector3</mark>
            <div class="descr">The pivot position of the rotation.</div>
            <div class="alias"><mark class="markstyle">pivot</mark>, <mark class="markstyle">pv</mark>, <mark class="markstyle">p</mark></div></li>
            <li> StartAngle: <mark class="markstyle">Vector3</mark>
            <div class="descr">The starting euler angles.</div>
            <div class="alias"><mark class="markstyle">startangle</mark>, <mark class="markstyle">start</mark></div></li>
            <li> TargetAngle: <mark class="markstyle">Vector3</mark>
            <div class="descr">The target euler angles.</div>
            <div class="alias"><mark class="markstyle">targetangle</mark>, <mark class="markstyle">target</mark></div></li>
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
            <li> Duration: <mark class="markstyle">float</mark>
            <div class="descr">How long the animation will take to fully show / hide the character.</div>
            <div class="alias"><mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></div></li>
            <li> Curve: <mark class="markstyle">AnimationCurve</mark>
            <div class="descr">The curve used for getting the t-value to interpolate between the scales.</div>
            <div class="alias"><mark class="markstyle">curve</mark>, <mark class="markstyle">crv</mark>, <mark class="markstyle">c</mark></div></li>
            <li> <mark class="markstyle">Show</mark> StartScale: <mark class="markstyle">Vector3</mark>
            <div class="descr">The scale to start growing to the initial scale from.</div>
            <div class="alias"><mark class="markstyle">startscale</mark>, <mark class="markstyle">startscl</mark>, <mark class="markstyle">start</mark></div></li>
            <li> <mark class="markstyle">Hide</mark> TargetScale: <mark class="markstyle">Vector3</mark>
            <div class="descr">The scale to grow to from the initial scale.</div>
            <div class="alias"><mark class="markstyle">targetscale</mark>, <mark class="markstyle">targetscl</mark>, <mark class="markstyle">target</mark></div></li>
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
            <li> Duration: <mark class="markstyle">float</mark>
            <div class="descr">How long the animation will take to fully show / hide the character.</div>
            <div class="alias"><mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></div></li>
            <li> Curve: <mark class="markstyle">AnimationCurve</mark>
            <div class="descr">The curve used for getting the t-value to interpolate between the percentages.</div>
            <div class="alias"><mark class="markstyle">curve</mark>, <mark class="markstyle">crv</mark>, <mark class="markstyle">c</mark></div></li>
            <li> Anchor: <mark class="markstyle">TypedVector3</mark>
            <div class="descr">The anchor from where the character spreads.</div>
            <div class="alias"><mark class="markstyle">anchor</mark>, <mark class="markstyle">anc</mark>, <mark class="markstyle">a</mark></div></li>
            <li> Direction: <mark class="markstyle">Vector3</mark>
            <div class="descr">The direction in which the character spreads.</div>
            <div class="alias"><mark class="markstyle">direction</mark>, <mark class="markstyle">dir</mark>, <mark class="markstyle">d</mark></div></li>
            <li> StartPercentage: <mark class="markstyle">float</mark>
            <div class="descr">The start percentage of the spread, 0 being fully hidden.</div>
            <div class="alias"><mark class="markstyle">startpercentage</mark>, <mark class="markstyle">start</mark></div></li>
            <li> TargetPercentage: <mark class="markstyle">float</mark>
            <div class="descr">The target percentage of the spread, 1 being fully shown.</div>
            <div class="alias"><mark class="markstyle">targetpercentage</mark>, <mark class="markstyle">target</mark></div></li>
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
            <li> Duration: <mark class="markstyle">float</mark>
            <div class="descr">How long the animation will take to fully show / hide the character.</div>
            <div class="alias"><mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></div></li>
            <li> Curve: <mark class="markstyle">AnimationCurve</mark>
            <div class="descr">The curve used for getting the t-value to interpolate between the start and target position.</div>
            <div class="alias"><mark class="markstyle">curve</mark>, <mark class="markstyle">crv</mark>, <mark class="markstyle">c</mark></div></li>
            <li> <mark class="markstyle">Show</mark> StartPositon: <mark class="markstyle">TypedVector3</mark>
            <div class="descr">The postion to move the character in from.</div>
            <div class="alias"><mark class="markstyle">startposition</mark>, <mark class="markstyle">startpos</mark>, <mark class="markstyle">start</mark></div></li>
            <li> <mark class="markstyle">Hide</mark> TargetPosition: <mark class="markstyle">TypedVector3</mark>
            <div class="descr">The postion to move the character to.</div>
            <div class="alias"><mark class="markstyle">targetposition</mark>, <mark class="markstyle">targetpos</mark>, <mark class="markstyle">target</mark></div></li>
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
            <li> Duration: <mark class="markstyle">float</mark>
            <div class="descr">How long the animation will take to fully show / hide the character.</div>
            <div class="alias"><mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></div></li>
            <li> MaxXAmplitude: <mark class="markstyle">float</mark>
            <div class="descr">The maximum X amplitude of each shake.</div>
            <div class="alias"><mark class="markstyle">maxxamplitude</mark>, <mark class="markstyle">maxxamp</mark>, <mark class="markstyle">maxxa</mark>, <mark class="markstyle">maxx</mark></div></li>
            <li> MinXAmplitude: <mark class="markstyle">float</mark>
            <div class="descr">The minimum X amplitude of each shake.</div>
            <div class="alias"><mark class="markstyle">minxamplitude</mark>, <mark class="markstyle">minxamp</mark>, <mark class="markstyle">minxa</mark>, <mark class="markstyle">minx</mark></div></li>
            <li> MaxYAmplitude: <mark class="markstyle">float</mark>
            <div class="descr">The maximum Y amplitude of each shake.</div>
            <div class="alias"><mark class="markstyle">maxyamplitude</mark>, <mark class="markstyle">maxyamp</mark>, <mark class="markstyle">maxya</mark>, <mark class="markstyle">maxy</mark></div></li>
            <li> MinYAmplitude: <mark class="markstyle">float</mark>
            <div class="descr">The minimum Y amplitude of each shake.</div>
            <div class="alias"><mark class="markstyle">minyamplitude</mark>, <mark class="markstyle">minyamp</mark>, <mark class="markstyle">minya</mark>, <mark class="markstyle">miny</mark></div></li>
            <li> MaxWait: <mark class="markstyle">float</mark>
            <div class="descr">The minimum amount of time to wait after each shake.</div>
            <div class="alias"><mark class="markstyle">maxwait</mark>, <mark class="markstyle">maxw</mark></div></li>
            <li> MinWait: <mark class="markstyle">float</mark>
            <div class="descr">The maximum amount of time to wait after each shake.</div>
            <div class="alias"><mark class="markstyle">minwait</mark>, <mark class="markstyle">minw</mark></div></li>
            <li> WaitCurve: <mark class="markstyle">AnimationCurve</mark>
            <div class="descr">The curve that defines the falloff of the wait between each shake.</div>
            <div class="alias"><mark class="markstyle">waitcurve</mark>, <mark class="markstyle">waitcrv</mark>, <mark class="markstyle">waitc</mark></div></li>
            <li> AmplitudeCurve: <mark class="markstyle">AnimationCurve</mark>
            <div class="descr">The curve that defines the falloff of the amplitude of each shake.</div>
            <div class="alias"><mark class="markstyle">amplitudecurve</mark>, <mark class="markstyle">amplitudecrv</mark>, <mark class="markstyle">amplitudec</mark>, <mark class="markstyle">ampcurve</mark>, <mark class="markstyle">ampcrv</mark>, <mark class="markstyle">ampc</mark></div></li>
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
            <li> Duration: <mark class="markstyle">float</mark>
            <div class="descr">How long the animation will take to fully show / hide the character.</div>
            <div class="alias"><mark class="markstyle">duration</mark>, <mark class="markstyle">dur</mark>, <mark class="markstyle">d</mark></div></li>
            <li> Characters: <mark class="markstyle">string</mark>
            <div class="descr">The pool of characters to change to.</div>
            <div class="alias"><mark class="markstyle">characters</mark>, <mark class="markstyle">chars</mark>, <mark class="markstyle">char</mark>, <mark class="markstyle">c</mark></div></li>
            <li> Probability: <mark class="markstyle">float</mark>
            <div class="descr">The probability to change to a character different from the original.</div>
            <div class="alias"><mark class="markstyle">probability</mark>, <mark class="markstyle">prob</mark>, <mark class="markstyle">p</mark></div></li>
            <li> MinWait: <mark class="markstyle">float</mark>
            <div class="descr">The minimum amount of time to wait once a character changed (or did not change).</div>
            <div class="alias"><mark class="markstyle">minwait</mark>, <mark class="markstyle">minw</mark>, <mark class="markstyle">min</mark></div></li>
            <li> MaxWait: <mark class="markstyle">float</mark>
            <div class="descr">The maximum amount of time to wait once a character changed (or did not change).</div>
            <div class="alias"><mark class="markstyle">maxwait</mark>, <mark class="markstyle">maxw</mark>, <mark class="markstyle">max</mark></div></li>
            <li> AutoCase: <mark class="markstyle">bool</mark>
            <div class="descr">Whether to ensure capitalized characters are only changed to other capitalized characters, and vice versa.</div>
            <div class="alias"><mark class="markstyle">autocase</mark>, <mark class="markstyle">case</mark></div></li>
            <li> WaitCurve: <mark class="markstyle">AnimationCurve</mark>
            <div class="descr">The curve that defines the falloff of the wait between each change.</div>
            <div class="alias"><mark class="markstyle">waitcurve</mark>, <mark class="markstyle">waitcrv</mark>, <mark class="markstyle">waitc</mark></div</li>
            <li> ProbabilityCurve: <mark class="markstyle">AnimationCurve</mark>
            <div class="descr">The curve that defines the falloff of the probability of changing to a character other than the original.</div>
            <div class="alias"><mark class="markstyle">probabilitycurve</mark>, <mark class="markstyle">probabilitycrv</mark>, <mark class="markstyle">probabilityc</mark>, <mark class="markstyle">probcurve</mark>, <mark class="markstyle">probcrv</mark>, <mark class="markstyle">probc</mark></div</li>
        </ul>
    </div>
  </div>
</div>
</div>
</div>

<div></div>
<div></div>
</div>
