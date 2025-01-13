<link rel="stylesheet" type="text/css" href="../styles.css">

# Built-in animations

This section gives you a complete overview of all built-in basic animations and their parameters (for show / hide / scene animations see the respective sections).
Basic animations are those type of animation seen in the previous section, which animate a piece of text continuously over time.  
You apply basic animations like you would a normal TextMeshPro tag, e.g.: <mark class="markstyle">&lt;wave&gt;TMPEffects&lt;/&gt;</mark>.  

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
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/wave.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblewave" class="toggle" type="checkbox">
  <label for="collapsiblewave" class="lbl-toggle">wave Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
        <p>
        <ul>
            <li> Wave: see <a href="tmpanimator_animationutility_wave.html">Waves</a></li>
            <li> OffsetProvider: see <a href="offsetproviders.html">OffsetProviders</a></li>
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
            <li> Wave: see <a href="tmpanimator_animationutility_wave.html">Waves</a></li>
            <li> OffsetProvider: see <a href="offsetproviders.html">OffsetProviders</a></li>
            <li> MaxOpacity: <mark class="markstyle">float</mark>
            <div class="descr">The maximum opacity.</div>
            <div class="alias"><mark class="markstyle">maxopacity</mark>, <mark class="markstyle">maxop</mark>, <mark class="markstyle">max</mark></div></li>
            <li> FadeInAnchor: <mark class="markstyle">Vector3</mark>
            <div class="descr">The anchor used for fading in.</div>
            <div class="alias"><mark class="markstyle">fadeinanchor</mark>, <mark class="markstyle">fianchor</mark>, <mark class="markstyle">fianc</mark>, <mark class="markstyle">fia</mark></div></li>
            <li> FadeInDirection: <mark class="markstyle">Vector3</mark>
            <div class="descr">The direction used for fading in.</div>
            <div class="alias"><mark class="markstyle">fadeindirection</mark>, <mark class="markstyle">fidirection</mark>, <mark class="markstyle">fidir</mark>, <mark class="markstyle">fid</mark></div></li>
            <li> MinOpacity: <mark class="markstyle">float</mark>
            <div class="descr">The minimum opacity.</div>
            <div class="alias"><mark class="markstyle">minopacity</mark>, <mark class="markstyle">minop</mark>, <mark class="markstyle">min</mark></div></li>
            <li> FadeOutAnchor: <mark class="markstyle">Vector3</mark>
            <div class="descr">The anchor used for fading out.</div>
            <div class="alias"><mark class="markstyle">fadeoutanchor</mark>, <mark class="markstyle">foanchor</mark>, <mark class="markstyle">foanc</mark>, <mark class="markstyle">foa</mark></div></li>
            <li> FadeOutDirection: <mark class="markstyle">Vector3</mark>         
            <div class="descr">The direction used for fading out.</div>
            <div class="alias"><mark class="markstyle">fadeoutdirection</mark>, <mark class="markstyle">fodirection</mark>, <mark class="markstyle">fodir</mark>, <mark class="markstyle">fod</mark></div></li>  
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
            <li> Wave: see <a href="tmpanimator_animationutility_wave.html">Waves</a></li>
            <li> OffsetProvider: see <a href="offsetproviders.html">OffsetProviders</a></li>
            <li> Pivot: <mark class="markstyle">TypedVector3</mark>
            <div class="descr">The pivot of the rotation.</div>
            <div class="alias"><mark class="markstyle">pivot</mark>, <mark class="markstyle">pv</mark>, <mark class="markstyle">p</mark></div></li>  
            <li> RotationAxis: <mark class="markstyle">Vector3</mark>
            <div class="descr">The axis to rotate around.</div>
            <div class="alias"><mark class="markstyle">rotationaxis</mark>, <mark class="markstyle">axis</mark>, <mark class="markstyle">a</mark></div></li>  
            <li> MaxAngleLimit: <mark class="markstyle">float</mark>
            <div class="descr">The maximum angle.</div>
            <div class="alias"><mark class="markstyle">maxangle</mark>, <mark class="markstyle">maxa</mark>, <mark class="markstyle">max</mark></div></li>  
            <li> MinAngleLimit: <mark class="markstyle">Vector3</mark>            
            <div class="descr">The minimum angle.</div>
            <div class="alias"><mark class="markstyle">minangle</mark>, <mark class="markstyle">mina</mark>, <mark class="markstyle">min</mark></div></li>  
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
            <li> Speed: <mark class="markstyle">float</mark>
            <div class="descr">The speed at which the animation plays.</div>
            <div class="alias"><mark class="markstyle">speed</mark>, <mark class="markstyle">sp</mark>, <mark class="markstyle">s</mark></div></li>
            <li> SqueezeFactor: <mark class="markstyle">float</mark>
            <div class="descr">The percentage of its original size the text is squeezed to.</div>
            <div class="alias"><mark class="markstyle">squeezefactor</mark>, <mark class="markstyle">squeeze</mark>, <mark class="markstyle">sqz</mark></div></li>
            <li> Amplitude: <mark class="markstyle">float</mark>
            <div class="descr">The amplitude the text pushes to the left / right.</div>
            <div class="alias"><mark class="markstyle">amplitude</mark>, <mark class="markstyle">amp</mark></div></li>
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
  <label for="collapsiblechar" class="lbl-toggle">char Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
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
            <li> Uniform: <mark class="markstyle">bool</mark>
            <div class="descr">Whether to apply the shake uniformly across the text.</div>
            <div class="alias"><mark class="markstyle">uniform</mark>, <mark class="markstyle">uni</mark></div></li>
            <li> MaxXAmplitude: <mark class="markstyle">float</mark>
            <div class="descr">The maximum X amplitude of each shake.</div>
            <div class="alias"><mark class="markstyle">maxxamplitude</mark>, <mark class="markstyle">maxxamp</mark>, <mark class="markstyle">maxxa</mark>, <mark class="markstyle">maxx</mark></div></li>
            <li> MinXAmplitude: <mark class="markstyle">float</mark>
            <div class="descr">The minimum X amplitude of each shake.</div>
            <div class="descr"><mark class="markstyle">minxamplitude</mark>, <mark class="markstyle">minxamp</mark>, <mark class="markstyle">minxa</mark>, <mark class="markstyle">minx</mark></div></li>
            <li> MaxYAmplitude: <mark class="markstyle">float</mark>
            <div class="descr">The maximum Y amplitude of each shake.</div>
            <div class="alias"><mark class="markstyle">maxyamplitude</mark>, <mark class="markstyle">maxyamp</mark>, <mark class="markstyle">maxya</mark>, <mark class="markstyle">maxy</mark></div></li>
            <li> MinYAmplitude: <mark class="markstyle">float</mark>
            <div class="descr">The minimum Y amplitude of each shake.</div>
            <div class="alias"><mark class="markstyle">minyamplitude</mark>, <mark class="markstyle">minyamp</mark>, <mark class="markstyle">minya</mark>, <mark class="markstyle">miny</mark></div></li>
            <li> UniformWait: <mark class="markstyle">bool</mark>
            <div class="descr">Whether to use uniform wait time across the text. Ignored if uniform is true.</div>
            <div class="alias"><mark class="markstyle">uniformwait</mark>, <mark class="markstyle">uniwait</mark>, <mark class="markstyle">uniw</mark></div></li>
            <li> MaxWait: <mark class="markstyle">float</mark>
            <div class="descr">The minimum amount of time to wait after each shake.</div>
            <div class="alias"><mark class="markstyle">maxwait</mark>, <mark class="markstyle">maxw</mark></div></li>
            <li> MinWait: <mark class="markstyle">float</mark>
            <div class="descr">The maximum amount of time to wait after each shake.</div>
            <div class="alias"><mark class="markstyle">minwait</mark>, <mark class="markstyle">minw</mark></div></li>
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
            <li> Wave: see <a href="tmpanimator_animationutility_wave.html">Waves</a></li>
            <li> OffsetProvider: see <a href="offsetproviders.html">OffsetProviders</a></li>
            <li> MaxScale: <mark class="markstyle">float</mark>
            <div class="descr">The maximum scale to grow to.</div>
            <div class="alias"><mark class="markstyle">maxscale</mark>, <mark class="markstyle">maxscl</mark>, <mark class="markstyle">max</mark></div</li>
            <li> MinScale: <mark class="markstyle">float</mark>
            <div class="descr">The minimum scale to shrink to.</div>
            <div class="alias"><mark class="markstyle">minscale</mark>, <mark class="markstyle">minscl</mark>, <mark class="markstyle">min</mark></div></li>
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
  <label for="collapsiblepalette" class="lbl-toggle">palette Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
            <li> Wave: see <a href="tmpanimator_animationutility_wave.html">Waves</a></li>
            <li> OffsetProvider: see <a href="offsetproviders.html">OffsetProviders</a></li>
            <li> Colors: <mark class="markstyle">Color[]</mark>
            <div class="descr">The colors to cycle through.</div>
            <div class="alias"><mark class="markstyle">colors</mark>, <mark class="markstyle">clrs</mark></div></li>
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
            <li> Wave: see <a href="tmpanimator_animationutility_wave.html">Waves</a></li>
            <li> OffsetProvider: see <a href="offsetproviders.html">OffsetProviders</a></li>
            <li> GrowAnchor: <mark class="markstyle">TypedVector3</mark>
            <div class="descr">The anchor used for growing.</div>
            <div class="alias"><mark class="markstyle">growanchor</mark>, <mark class="markstyle">growanc</mark>, <mark class="markstyle">ganc</mark></div></li>
            <li> GrowDirection: <mark class="markstyle">Vector3</mark>
            <div class="descr">The direction used for growing.</div>
            <div class="alias"><mark class="markstyle">growdirection</mark>, <mark class="markstyle">growdir</mark>, <mark class="markstyle">gdir</mark></div></li>
            <li> ShrinkAnchor: <mark class="markstyle">TypedVector3</mark>
            <div class="descr">The anchor used for shrinking.</div>
            <div class="alias"><mark class="markstyle">shrinkanchor</mark>, <mark class="markstyle">shrinkanc</mark>, <mark class="markstyle">sanc</mark></div></li>
            <li> ShrinkDirection: <mark class="markstyle">Vector3</mark>
            <div class="descr">The direction used for shrinking.</div>
            <div class="alias"><mark class="markstyle">shrinkdirection</mark>, <mark class="markstyle">shrinkdir</mark>, <mark class="markstyle">sdir</mark></div></li>
            <li> MaxPercentage: <mark class="markstyle">float</mark>
            <div class="descr">The maximum percentage to spread to, at 1 being completely shown.</div>
            <div class="alias"><mark class="markstyle">maxpercentage</mark>, <mark class="markstyle">maxp</mark>, <mark class="markstyle">max</mark></div></li>
            <li> MinPercentage: <mark class="markstyle">float</mark>
            <div class="descr">The minimum percentage to unspread to, at 0 being completely hidden.</div>
            <div class="alias"><mark class="markstyle">minpercentage</mark>, <mark class="markstyle">minp</mark>, <mark class="markstyle">min</mark></div></li>
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
          <li> Pivot: <mark class="markstyle">TypedVector3</mark>
          <div class="descr">The pivot position of the rotation.</div>  
          <div class="alias"><mark class="markstyle">pivot</mark>, <mark class="markstyle">pv</mark>, <mark class="markstyle">p</mark></div></li>
          <li> RotationAxis: <mark class="markstyle">Vector3</mark>
          <div class="descr">The axis to rotate around.</div>  
          <div class="alias"><mark class="markstyle">rotationaxis</mark>, <mark class="markstyle">axis</mark>, <mark class="markstyle">a</mark></div></li>
          <li> Speed: <mark class="markstyle">float</mark>
          <div class="descr">The speed of the rotation, in rotations per second.</div>  
          <div class="alias"><mark class="markstyle">speed</mark>, <mark class="markstyle">sp</mark>, <mark class="markstyle">s</mark></div></li>
          </ul>
      </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/shear.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsibleshear" class="toggle" type="checkbox">
  <label for="collapsibleshear" class="lbl-toggle">shear Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
            <li> Wave: see <a href="tmpanimator_animationutility_wave.html">Waves</a></li>
            <li> OffsetProvider: see <a href="offsetproviders.html">OffsetProviders</a></li>
        </ul>
      </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/dangle.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsibledangle" class="toggle" type="checkbox">
  <label for="collapsibledangle" class="lbl-toggle">dangle Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
        <ul>
            <li> Wave: see <a href="tmpanimator_animationutility_wave.html">Waves</a></li>
            <li> OffsetProvider: see <a href="offsetproviders.html">OffsetProviders</a></li>
        </ul>
      </p>
    </div>
  </div>
</div>
</div>

<div class="anim-container">
<div class="wrap-collabsible">
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/basic/sketchy.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

  <input id="collapsiblesketchy" class="toggle" type="checkbox">
  <label for="collapsiblesketchy" class="lbl-toggle">sketchy Parameters</label>
  <div class="collapsible-content">
    <div class="content-inner">
      <p>
      <ul>
          <li> Delay: <mark class="markstyle">float</mark>
          <div class="descr">The delay between each change, in seconds.</div>  
          <div class="alias"><mark class="markstyle">delay</mark>, <mark class="markstyle">d</mark></div></li>
          <li> Min/MaxOffset: <mark class="markstyle">Vector3</mark>
          <div class="descr">The min/max offset from the original position.</div>  
          <div class="alias"><mark class="markstyle">minoffset / maxoffset</mark>, <mark class="markstyle">minoff / maxoff</mark></div></li>
          <li> Min/MaxRotation: <mark class="markstyle">Vector3</mark>
          <div class="descr">The min/max rotation.</div>  
          <div class="alias"><mark class="markstyle">minrotation / maxrotation</mark>, <mark class="markstyle">minrot / maxrot</mark></div></li>
          <li> Min/MaxScale: <mark class="markstyle">Vector3</mark>
          <div class="descr">The min/max scale.</div>  
          <div class="alias"><mark class="markstyle">minscale / maxscale</mark>, <mark class="markstyle">minscl / maxscl</mark></div></li>
          <li> Min/MaxColorShift: <mark class="markstyle">Vector3</mark>
          <div class="descr">The min/max color shift, as RGB values.</div>  
          <div class="alias"><mark class="markstyle">mincolorshift / maxcolorshift</mark>, <mark class="markstyle">minclrshift / maxclrshift</mark>, <mark class="markstyle">minclr / maxclr</mark></div></li>
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
