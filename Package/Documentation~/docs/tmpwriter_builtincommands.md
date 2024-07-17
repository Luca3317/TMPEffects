# Getting started with TMPWriter
This section gives you a complete overview of all built-in commands.
All of the built-in commands modify the TMPWriter's writing behavior (with the exception of <mark style="color: lightgray; background-color: #191a18">&lt;!debug=""&gt;</mark>).
<mark style="color: lightgray; background-color: #191a18">SceneCommands</mark> allow you to call any method you want.

<style>
.anim-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
}

.anim-grid > div {
    margin-bottom: 1.2rem;
    margin-top: 1.2rem;
}
</style>

<div class="anim-grid">

<div style="display: inline-block">
<b>Wait</b> - Pause the writer for the given amount of time
<hr>
<p>
Parameters:
      <mark style="color: lightgray; background-color: #191a18">name</mark> : time in seconds

Example:
      <mark style="color: lightgray; background-color: #191a18">I WILL NOW &lt;!wait=1.5&gt;WAIT</mark>
</p>
</div>

<div>
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/writetextnoanim2.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>
</div>



<div style="display: inline-block">
<b>Show</b> - Show the text block from the very start
<hr>
<p>
Parameters:
      None

Example:
      <mark style="color: lightgray; background-color: #191a18">THIS WILL ALWAYS BE &lt;!show&gt;SHOWN&lt;/!show&gt;, FROM THE VERY START</mark>
</p>
</div>

<div>
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/commands/show.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>
</div>




<div style="display: inline-block">
<b>Delay</b> - Set the delay between showing characters
<hr>
<p>
Parameters:
      <mark style="color: lightgray; background-color: #191a18">name</mark> : delay in seconds

Example:
      <mark style="color: lightgray; background-color: #191a18">&lt;!delay=0.25&gt;I START SLOW...&lt;!delay=0.05&gt;BUT NOW IM FAST</mark>
</p>
</div>

<div>
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/commands/delay.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>
</div>



<div style="display: inline-block">
<b>Skippable</b> - Set whether the text is skippable
<hr>
<p>
Parameters:
      <mark style="color: lightgray; background-color: #191a18">name</mark> : <mark style="color: lightgray; background-color: #191a18">true</mark>/<mark style="color: lightgray; background-color: #191a18">false</mark>

Example:
      <mark style="color: lightgray; background-color: #191a18">&lt;!skippable=true&gt;WHEN IM SKIPPED, I WONT &lt;!skippable=false&gt;GO ALL THE WAY</mark>
</p>
</div>

<div>
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/previews/commands/skippable.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>
</div>



<div style="display: inline-block">
<b>Debug</b> - Print a message to the Unity console; you hopefully wont need this much but I decided to leave it in anyway &#128516;
<hr>
<p>
Parameters:
      <mark style="color: lightgray; background-color: #191a18">name</mark> : Your message
      <mark style="color: lightgray; background-color: #191a18">type</mark> : <mark style="color: lightgray; background-color: #191a18">l(og)</mark> / <mark style="color: lightgray; background-color: #191a18">w(arning)</mark> / <mark style="color: lightgray; background-color: #191a18">e(rror)</mark>

Example:
      <mark style="color: lightgray; background-color: #191a18">PRINTING TO THE CONSOLE NOW:&lt;!debug="Test Message" type="warning"&gt;</mark>
</p>
</div>

</div>