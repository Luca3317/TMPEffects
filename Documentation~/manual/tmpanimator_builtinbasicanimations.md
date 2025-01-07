<link rel="stylesheet" type="text/css" href="../styles.css">

# Built-in animations

This section gives you a complete overview of all built-in basic animations and their parameters (for show / hide / scene animations see the respective sections).
Basic animations are those type of animation seen in the previous section, which animate a piece of text continuously over time.  
You apply basic animations like you would a normal TextMeshPro tag, e.g.: <mark class="markstyle">&lt;wave&gt;TMPEffects&lt;/&gt;</mark>.

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
            <mark class="markstyle">waveoffset</mark>, <mark class="markstyle">woffset</mark>, <mark class="markstyle">waveoff</mark>, <mark class="markstyle">woff</mark></li>
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
            <mark class="markstyle">waveoffset</mark>, <mark class="markstyle">woffset</mark>, <mark class="markstyle">waveoff</mark>, <mark class="markstyle">woff</mark></li>
            <li> MaxOpacity: The maximum opacity that is reached.<br>
            <mark class="markstyle">maxopacity</mark>, <mark class="markstyle">maxop</mark>, <mark class="markstyle">max</mark></li>
            <li> FadeInAnchor: The anchor used for fading in.<br>
            <mark class="markstyle">fadeinanchor</mark>, <mark class="markstyle">fianchor</mark>, <mark class="markstyle">fianc</mark>, <mark class="markstyle">fia</mark></li>
            <li> FadeInDirection: The direction used for fading in.<br>
            <mark class="markstyle">fadeindirection</mark>, <mark class="markstyle">fidirection</mark>, <mark class="markstyle">fidir</mark>, <mark class="markstyle">fid</mark></li>
            <li> MinOpacity: The minimum opacity that is reached.<br>
            <mark class="markstyle">minopacity</mark>, <mark class="markstyle">minop</mark>, <mark class="markstyle">min</mark></li>
            <li> FadeOutAnchor: The anchor used for fading out.<br>
            <mark class="markstyle">fadeoutanchor</mark>, <mark class="markstyle">foanchor</mark>, <mark class="markstyle">foanc</mark>, <mark class="markstyle">foa</mark></li>
            <li> FadeOutDirection: The direction used for fading out.<br>
            <mark class="markstyle">fadeoutdirection</mark>, <mark class="markstyle">fodirection</mark>, <mark class="markstyle">fodir</mark>, <mark class="markstyle">fod</mark></li>
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
            <mark class="markstyle">waveoffset</mark>, <mark class="markstyle">woffset</mark>, <mark class="markstyle">waveoff</mark>, <mark class="markstyle">woff</mark></li>
            <li> Pivot: The pivot position of the rotation.<br>
            <mark class="markstyle">pivot</mark>, <mark class="markstyle">pv</mark>, <mark class="markstyle">p</mark></li>
            <li> RotationAxis: The axis to rotate around.<br>
            <mark class="markstyle">rotationaxis</mark>, <mark class="markstyle">axis</mark>, <mark class="markstyle">a</mark>
            <li> MaxAngleLimit: The maximum angle of the rotation.<br>
            <mark class="markstyle">maxangle</mark>, <mark class="markstyle">maxa</mark>, <mark class="markstyle">max</mark></li>
            <li> MinAngleLimit: The minimum angle of the rotation.<br>
            <mark class="markstyle">minangle</mark>, <mark class="markstyle">mina</mark>, <mark class="markstyle">min</mark></li>
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
            <mark class="markstyle">speed</mark>, <mark class="markstyle">sp</mark>, <mark class="markstyle">s</mark></li>
            <li> SqueezeFactor: The percentage of its original size the text is squeezed to.<br>
            <mark class="markstyle">squeezefactor</mark>, <mark class="markstyle">squeeze</mark>, <mark class="markstyle">sqz</mark></li>
            <li> Amplitude: The amplitude the text pushes to the left / right.<br>
            <mark class="markstyle">amplitude</mark>, <mark class="markstyle">amp</mark>
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
            <mark class="markstyle">characters</mark>, <mark class="markstyle">chars</mark>, <mark class="markstyle">char</mark>, <mark class="markstyle">c</mark></li>
            <li> Probability: The probability to change to a character different from the original.<br>
            <mark class="markstyle">probability</mark>, <mark class="markstyle">prob</mark>, <mark class="markstyle">p</mark></li>
            <li> MinWait: The minimum amount of time to wait once a character changed (or did not change).<br>
            <mark class="markstyle">minwait</mark>, <mark class="markstyle">minw</mark>, <mark class="markstyle">min</mark>
            <li> MaxWait: The maximum amount of time to wait once a character changed (or did not change).<br>
            <mark class="markstyle">maxwait</mark>, <mark class="markstyle">maxw</mark>, <mark class="markstyle">max</mark>
            <li> AutoCase: Whether to ensure capitalized characters are only changed to other capitalized characters, and vice versa.<br>
            <mark class="markstyle">autocase</mark>, <mark class="markstyle">case</mark>
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
            <mark class="markstyle">uniform</mark>, <mark class="markstyle">uni</mark>
            <li> MaxXAmplitude: The maximum X amplitude of each shake.<br>
            <mark class="markstyle">maxxamplitude</mark>, <mark class="markstyle">maxxamp</mark>, <mark class="markstyle">maxxa</mark>, <mark class="markstyle">maxx</mark></li>
            <li> MinXAmplitude: The minimum X amplitude of each shake.<br>
            <mark class="markstyle">minxamplitude</mark>, <mark class="markstyle">minxamp</mark>, <mark class="markstyle">minxa</mark>, <mark class="markstyle">minx</mark></li>
            <li> MaxYAmplitude: The maximum Y amplitude of each shake.<br>
            <mark class="markstyle">maxyamplitude</mark>, <mark class="markstyle">maxyamp</mark>, <mark class="markstyle">maxya</mark>, <mark class="markstyle">maxy</mark></li>
            <li> MinYAmplitude: The minimum Y amplitude of each shake.<br>
            <mark class="markstyle">minyamplitude</mark>, <mark class="markstyle">minyamp</mark>, <mark class="markstyle">minya</mark>, <mark class="markstyle">miny</mark></li>
            <li> UniformWait: Whether to use uniform wait time across the text. Ignored if uniform is true.<br>
            <mark class="markstyle">uniformwait</mark>, <mark class="markstyle">uniwait</mark>, <mark class="markstyle">uniw</mark></li>
            <li> MaxWait: The minimum amount of time to wait after each shake.<br>
            <mark class="markstyle">maxwait</mark>, <mark class="markstyle">maxw</mark></li>
            <li> MinWait: The maximum amount of time to wait after each shake.<br>
            <mark class="markstyle">minwait</mark>, <mark class="markstyle">minw</mark></li>
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
            <mark class="markstyle">waveoffset</mark>, <mark class="markstyle">woffset</mark>, <mark class="markstyle">waveoff</mark>, <mark class="markstyle">woff</mark></li>
            <li> MaxScale: The maximum scale to grow to.<br>
            <mark class="markstyle">maxscale</mark>, <mark class="markstyle">maxscl</mark>, <mark class="markstyle">max</mark></li>
            <li> MinScale: The minimum scale to shrink to.<br>
            <mark class="markstyle">minscale</mark>, <mark class="markstyle">minscl</mark>, <mark class="markstyle">min</mark></li>
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
            <mark class="markstyle">waveoffset</mark>, <mark class="markstyle">woffset</mark>, <mark class="markstyle">waveoff</mark>, <mark class="markstyle">woff</mark></li>
            <li> Colors: The colors to cycle through.<br>
            <mark class="markstyle">colors</mark>, <mark class="markstyle">clrs</mark></li>
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
            <mark class="markstyle">waveoffset</mark>, <mark class="markstyle">woffset</mark>, <mark class="markstyle">waveoff</mark>, <mark class="markstyle">woff</mark></li>
            <li> GrowAnchor: The anchor used for growing.<br>
            <mark class="markstyle">growanchor</mark>, <mark class="markstyle">growanc</mark>, <mark class="markstyle">ganc</mark></li>
            <li> GrowDirection: The direction used for growing.<br>
            <mark class="markstyle">growdirection</mark>, <mark class="markstyle">growdir</mark>, <mark class="markstyle">gdir</mark></li>
            <li> ShrinkAnchor: The anchor used for shrinking.<br>
            <mark class="markstyle">shrinkanchor</mark>, <mark class="markstyle">shrinkanc</mark>, <mark class="markstyle">sanc</mark></li>
            <li> ShrinkDirection: The direction used for shrinking.<br>
            <mark class="markstyle">shrinkdirection</mark>, <mark class="markstyle">shrinkdir</mark>, <mark class="markstyle">sdir</mark></li>
            <li> MaxPercentage: The maximum percentage to spread to, at 1 being completely shown.<br>
            <mark class="markstyle">maxpercentage</mark>, <mark class="markstyle">maxp</mark>, <mark class="markstyle">max</mark></li>
            <li> MinPercentage: The minimum percentage to unspread to, at 0 being completely hidden.<br>
            <mark class="markstyle">minpercentage</mark>, <mark class="markstyle">minp</mark>, <mark class="markstyle">min</mark></li>
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
          <mark class="markstyle">pivot</mark>, <mark class="markstyle">pv</mark>, <mark class="markstyle">p</mark></li>
          <li> RotationAxis: The axis to rotate around.<br>  
          <mark class="markstyle">rotationaxis</mark>, <mark class="markstyle">axis</mark>, <mark class="markstyle">a</mark></li>
          <li> Speed: The speed of the rotation, in rotations per second.<br>  
          <mark class="markstyle">speed</mark>, <mark class="markstyle">sp</mark>, <mark class="markstyle">s</mark></li>
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
<mark class="markstyle">&lt;swing&gt;</mark> and <mark class="markstyle">&lt;jump&gt;</mark> are based on previous animations; they use the same code with different default values. <mark class="markstyle">&lt;swing&gt;</mark> is based on <mark class="markstyle">&lt;pivot&gt;</mark>, <mark class="markstyle">&lt;jump&gt;</mark> is based on <mark class="markstyle">&lt;wave&gt;</mark>. The parameters are therefore identical with the ones they are based on.
