# Built-in animations

This section gives you a complete overview of all built-in basic animations and their parameters (for show / hide / scene animations see the respective sections).
Basic animations are those type of animation seen in the previous section, which animate a piece of text continuously over time.

<style>

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
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/wave.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblewave" class="toggle" type="checkbox">
  <label for="collapsiblewave" class="lbl-toggle">wave Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
        <p>
        <ul>
            <li> Wave: see <a href="parameterutility.html#waves">ParameterUtility.Waves</a></li>
            <li> WaveOffsetType: The way the offset for the wave is calculated.<br>
            <mark style="color: lightgray; background-color: #191a18">waveoffset</mark>, <mark style="color: lightgray; background-color: #191a18">woffset</mark>, <mark style="color: lightgray; background-color: #191a18">waveoff</mark>, <mark style="color: lightgray; background-color: #191a18">woff</mark></li>
            </ul>
        </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/fade.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblefade" class="toggle" type="checkbox">
  <label for="collapsiblefade" class="lbl-toggle">Fade Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
            <li> Wave: see <a href="parameterutility.html#waves">ParameterUtility.Waves</a></li>
            <li> WaveOffsetType: The way the offset for the wave is calculated.<br>
            <mark style="color: lightgray; background-color: #191a18">waveoffset</mark>, <mark style="color: lightgray; background-color: #191a18">woffset</mark>, <mark style="color: lightgray; background-color: #191a18">waveoff</mark>, <mark style="color: lightgray; background-color: #191a18">woff</mark></li>
            <li> MaxOpacity: The maximum opacity that is reached.<br>
            <mark style="color: lightgray; background-color: #191a18">maxopacity</mark>, <mark style="color: lightgray; background-color: #191a18">maxop</mark>, <mark style="color: lightgray; background-color: #191a18">max</mark></li>
            <li> FadeInAnchor: The anchor used for fading in.<br>
            <mark style="color: lightgray; background-color: #191a18">fadeinanchor</mark>, <mark style="color: lightgray; background-color: #191a18">fianchor</mark>, <mark style="color: lightgray; background-color: #191a18">fianc</mark>, <mark style="color: lightgray; background-color: #191a18">fia</mark></li>
            <li> FadeInDirection: The direction used for fading in.<br>
            <mark style="color: lightgray; background-color: #191a18">fadeindirection</mark>, <mark style="color: lightgray; background-color: #191a18">fidirection</mark>, <mark style="color: lightgray; background-color: #191a18">fidir</mark>, <mark style="color: lightgray; background-color: #191a18">fid</mark></li>
            <li> MinOpacity: The minimum opacity that is reached.<br>
            <mark style="color: lightgray; background-color: #191a18">minopacity</mark>, <mark style="color: lightgray; background-color: #191a18">minop</mark>, <mark style="color: lightgray; background-color: #191a18">min</mark></li>
            <li> FadeOutAnchor: The anchor used for fading out.<br>
            <mark style="color: lightgray; background-color: #191a18">fadeoutanchor</mark>, <mark style="color: lightgray; background-color: #191a18">foanchor</mark>, <mark style="color: lightgray; background-color: #191a18">foanc</mark>, <mark style="color: lightgray; background-color: #191a18">foa</mark></li>
            <li> FadeOutDirection: The direction used for fading out.<br>
            <mark style="color: lightgray; background-color: #191a18">fadeoutdirection</mark>, <mark style="color: lightgray; background-color: #191a18">fodirection</mark>, <mark style="color: lightgray; background-color: #191a18">fodir</mark>, <mark style="color: lightgray; background-color: #191a18">fod</mark></li>
        </ul>
      </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video class="centered-video" style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/pivot.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>
  <input id="collapsiblepivot" class="toggle" type="checkbox">
  <label for="collapsiblepivot" class="lbl-toggle">Pivot Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
            <li> Wave: see <a href="parameterutility.html#waves">ParameterUtility.Waves</a></li>
            <li> WaveOffsetType: The way the offset for the wave is calculated.<br>
            <mark style="color: lightgray; background-color: #191a18">waveoffset</mark>, <mark style="color: lightgray; background-color: #191a18">woffset</mark>, <mark style="color: lightgray; background-color: #191a18">waveoff</mark>, <mark style="color: lightgray; background-color: #191a18">woff</mark></li>
            <li> Pivot: The pivot position of the rotation.<br>
            <mark style="color: lightgray; background-color: #191a18">pivot</mark>, <mark style="color: lightgray; background-color: #191a18">pv</mark>, <mark style="color: lightgray; background-color: #191a18">p</mark></li>
            <li> RotationAxis: The axis to rotate around.<br>
            <mark style="color: lightgray; background-color: #191a18">rotationaxis</mark>, <mark style="color: lightgray; background-color: #191a18">axis</mark>, <mark style="color: lightgray; background-color: #191a18">a</mark>
            <li> MaxAngleLimit: The maximum angle of the rotation.<br>
            <mark style="color: lightgray; background-color: #191a18">maxangle</mark>, <mark style="color: lightgray; background-color: #191a18">maxa</mark>, <mark style="color: lightgray; background-color: #191a18">max</mark></li>
            <li> MinAngleLimit: The minimum angle of the rotation.<br>
            <mark style="color: lightgray; background-color: #191a18">minangle</mark>, <mark style="color: lightgray; background-color: #191a18">mina</mark>, <mark style="color: lightgray; background-color: #191a18">min</mark></li>
        </ul>
      </p>
    </div>
  </div>
</div>
</div>


<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/funky.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblefunky" class="toggle" type="checkbox">
  <label for="collapsiblefunky" class="lbl-toggle">funky Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
        <p>
        <ul>
            <li> Speed: The speed at which the animation plays.<br>
            <mark style="color: lightgray; background-color: #191a18">speed</mark>, <mark style="color: lightgray; background-color: #191a18">sp</mark>, <mark style="color: lightgray; background-color: #191a18">s</mark></li>
            <li> SqueezeFactor: The percentage of its original size the text is squeezed to.<br>
            <mark style="color: lightgray; background-color: #191a18">squeezefactor</mark>, <mark style="color: lightgray; background-color: #191a18">squeeze</mark>, <mark style="color: lightgray; background-color: #191a18">sqz</mark></li>
            <li> Amplitude: The amplitude the text pushes to the left / right.<br>
            <mark style="color: lightgray; background-color: #191a18">amplitude</mark>, <mark style="color: lightgray; background-color: #191a18">amp</mark>
            </ul>
        </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/char.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblechar" class="toggle" type="checkbox">
  <label for="collapsiblechar" class="lbl-toggle">Fade Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
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
        </ul>
      </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/shake.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsibleshake" class="toggle" type="checkbox">
  <label for="collapsibleshake" class="lbl-toggle">shake Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
            <li> Uniform: Whether to apply the shake uniformly across the text.<br>
            <mark style="color: lightgray; background-color: #191a18">uniform</mark>, <mark style="color: lightgray; background-color: #191a18">uni</mark>
            <li> MaxXAmplitude: The maximum X amplitude of each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">maxxamplitude</mark>, <mark style="color: lightgray; background-color: #191a18">maxxamp</mark>, <mark style="color: lightgray; background-color: #191a18">maxxa</mark>, <mark style="color: lightgray; background-color: #191a18">maxx</mark></li>
            <li> MinXAmplitude: The minimum X amplitude of each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">minxamplitude</mark>, <mark style="color: lightgray; background-color: #191a18">minxamp</mark>, <mark style="color: lightgray; background-color: #191a18">minxa</mark>, <mark style="color: lightgray; background-color: #191a18">minx</mark></li>
            <li> MaxYAmplitude: The maximum Y amplitude of each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">maxyamplitude</mark>, <mark style="color: lightgray; background-color: #191a18">maxyamp</mark>, <mark style="color: lightgray; background-color: #191a18">maxya</mark>, <mark style="color: lightgray; background-color: #191a18">maxy</mark></li>
            <li> MinYAmplitude: The minimum Y amplitude of each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">minyamplitude</mark>, <mark style="color: lightgray; background-color: #191a18">minyamp</mark>, <mark style="color: lightgray; background-color: #191a18">minya</mark>, <mark style="color: lightgray; background-color: #191a18">miny</mark></li>
            <li> UniformWait: Whether to use uniform wait time across the text. Ignored if uniform is true.<br>
            <mark style="color: lightgray; background-color: #191a18">uniformwait</mark>, <mark style="color: lightgray; background-color: #191a18">uniwait</mark>, <mark style="color: lightgray; background-color: #191a18">uniw</mark></li>
            <li> MaxWait: The minimum amount of time to wait after each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">maxwait</mark>, <mark style="color: lightgray; background-color: #191a18">maxw</mark></li>
            <li> MinWait: The maximum amount of time to wait after each shake.<br>
            <mark style="color: lightgray; background-color: #191a18">minwait</mark>, <mark style="color: lightgray; background-color: #191a18">minw</mark></li>
        </ul>
      </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/grow.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblegrow" class="toggle" type="checkbox">
  <label for="collapsiblegrow" class="lbl-toggle">grow Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
        <p>
        <ul>
            <li> Wave: see <a href="parameterutility.html#waves">ParameterUtility.Waves</a></li>
            <li> WaveOffsetType: The way the offset for the wave is calculated.<br>
            <mark style="color: lightgray; background-color: #191a18">waveoffset</mark>, <mark style="color: lightgray; background-color: #191a18">woffset</mark>, <mark style="color: lightgray; background-color: #191a18">waveoff</mark>, <mark style="color: lightgray; background-color: #191a18">woff</mark></li>
            <li> MaxScale: The maximum scale to grow to.<br>
            <mark style="color: lightgray; background-color: #191a18">maxscale</mark>, <mark style="color: lightgray; background-color: #191a18">maxscl</mark>, <mark style="color: lightgray; background-color: #191a18">max</mark></li>
            <li> MinScale: The minimum scale to shrink to.<br>
            <mark style="color: lightgray; background-color: #191a18">minscale</mark>, <mark style="color: lightgray; background-color: #191a18">minscl</mark>, <mark style="color: lightgray; background-color: #191a18">min</mark></li>
            </ul>
        </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/palette.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblepalette" class="toggle" type="checkbox">
  <label for="collapsiblepalette" class="lbl-toggle">Fade Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
            <li> Wave: see <a href="parameterutility.html#waves">ParameterUtility.Waves</a></li>
            <li> WaveOffsetType: The way the offset for the wave is calculated.<br>
            <mark style="color: lightgray; background-color: #191a18">waveoffset</mark>, <mark style="color: lightgray; background-color: #191a18">woffset</mark>, <mark style="color: lightgray; background-color: #191a18">waveoff</mark>, <mark style="color: lightgray; background-color: #191a18">woff</mark></li>
            <li> Colors: The colors to cycle through.<br>
            <mark style="color: lightgray; background-color: #191a18">colors</mark>, <mark style="color: lightgray; background-color: #191a18">clrs</mark></li>
        </ul>
      </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/spread.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblespread" class="toggle" type="checkbox">
  <label for="collapsiblespread" class="lbl-toggle">spread Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
            <li> Wave: see <a href="parameterutility.html#waves">ParameterUtility.Waves</a></li>
            <li> WaveOffsetType: The way the offset for the wave is calculated.<br>
            <mark style="color: lightgray; background-color: #191a18">waveoffset</mark>, <mark style="color: lightgray; background-color: #191a18">woffset</mark>, <mark style="color: lightgray; background-color: #191a18">waveoff</mark>, <mark style="color: lightgray; background-color: #191a18">woff</mark></li>
            <li> GrowAnchor: The anchor used for growing.<br>
            <mark style="color: lightgray; background-color: #191a18">growanchor</mark>, <mark style="color: lightgray; background-color: #191a18">growanc</mark>, <mark style="color: lightgray; background-color: #191a18">ganc</mark></li>
            <li> GrowDirection: The direction used for growing.<br>
            <mark style="color: lightgray; background-color: #191a18">growdirection</mark>, <mark style="color: lightgray; background-color: #191a18">growdir</mark>, <mark style="color: lightgray; background-color: #191a18">gdir</mark></li>
            <li> ShrinkAnchor: The anchor used for shrinking.<br>
            <mark style="color: lightgray; background-color: #191a18">shrinkanchor</mark>, <mark style="color: lightgray; background-color: #191a18">shrinkanc</mark>, <mark style="color: lightgray; background-color: #191a18">sanc</mark></li>
            <li> ShrinkDirection: The direction used for shrinking.<br>
            <mark style="color: lightgray; background-color: #191a18">shrinkdirection</mark>, <mark style="color: lightgray; background-color: #191a18">shrinkdir</mark>, <mark style="color: lightgray; background-color: #191a18">sdir</mark></li>
            <li> MaxPercentage: The maximum percentage to spread to, at 1 being completely shown.<br>
            <mark style="color: lightgray; background-color: #191a18">maxpercentage</mark>, <mark style="color: lightgray; background-color: #191a18">maxp</mark>, <mark style="color: lightgray; background-color: #191a18">max</mark></li>
            <li> MinPercentage: The minimum percentage to unspread to, at 0 being completely hidden.<br>
            <mark style="color: lightgray; background-color: #191a18">minpercentage</mark>, <mark style="color: lightgray; background-color: #191a18">minp</mark>, <mark style="color: lightgray; background-color: #191a18">min</mark></li>
        </ul>
      </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/pivotc.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblepivotc" class="toggle" type="checkbox">
  <label for="collapsiblepivotc" class="lbl-toggle">pivotc Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
      <ul>
          <li> Pivot: The pivot position of the rotation.<br>  
          <mark style="color: lightgray; background-color: #191a18">pivot</mark>, <mark style="color: lightgray; background-color: #191a18">pv</mark>, <mark style="color: lightgray; background-color: #191a18">p</mark></li>
          <li> RotationAxis: The axis to rotate around.<br>  
          <mark style="color: lightgray; background-color: #191a18">rotationaxis</mark>, <mark style="color: lightgray; background-color: #191a18">axis</mark>, <mark style="color: lightgray; background-color: #191a18">a</mark></li>
          <li> Speed: The speed of the rotation, in rotations per second.<br>  
          <mark style="color: lightgray; background-color: #191a18">speed</mark>, <mark style="color: lightgray; background-color: #191a18">sp</mark>, <mark style="color: lightgray; background-color: #191a18">s</mark></li>
          </ul>
      </p>
    </div>
  </div>
</div>
</div>


<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/swing.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/jump.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>
</div>
</div>
</div>
</div>
</div>


<br>
<br>
<mark style="color: lightgray; background-color: #191a18">&lt;swing&gt;</mark> and <mark style="color: lightgray; background-color: #191a18">&lt;jump&gt;</mark> are based on previous animations; they use the same code with different default values. <mark style="color: lightgray; background-color: #191a18">&lt;swing&gt;</mark> is based on <mark style="color: lightgray; background-color: #191a18">&lt;pivot&gt;</mark>, <mark style="color: lightgray; background-color: #191a18">&lt;jump&gt;</mark> is based on <mark style="color: lightgray; background-color: #191a18">&lt;wave&gt;</mark>. The parameters are therefore identical with the ones they are based on.