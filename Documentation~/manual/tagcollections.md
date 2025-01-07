<link rel="stylesheet" type="text/css" href="../styles.css">

# TagCollections
<mark class="markstyle">TagCollection</mark> is a collection of <mark class="markstyle">TMPEffectTagTuples</mark>, which
combine [TMPEffectTags and TMPEffecTagIndices](effecttag.md).  
It maintains an order for the tags based on their indices, and exposes multiple functions to get contained tags based on their indices, and vice versa.

When creating a <mark class="markstyle">TagCollection</mark>, you can pass a <mark class="markstyle">[ITMPTagValidator](../api/TMPEffects.Tags.ITMPTagValidator.yml)</mark>, to guarantee you may only add specific tags to the collection.

For normal use cases, you won't be creating <mark class="markstyle">TagCollections</mark> though; generally, you will use the <mark class="markstyle">TagCollections</mark>
exposed by the TMPAnimator and TMPWriter to add or remove tags from script.

Be aware that if you add tags to a <mark class="markstyle">TagCollection</mark>, if there is already a tag present with the exact same <mark class="markstyle">StartIndex</mark> and <mark class="markstyle">OrderAtIndex</mark>, that the <mark class="markstyle">OrderAtIndex</mark> of the contained tags will be adjusted.