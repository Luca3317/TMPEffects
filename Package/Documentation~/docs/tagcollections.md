# TagCollections
<mark style="color: lightgray; background-color: #191a18">TagCollection</mark> is a collection of <mark style="color: lightgray; background-color: #191a18">TMPEffectTagTuples</mark>, which
combine [TMPEffectTags and TMPEffecTagIndices](effecttag.md).  
It maintains an order for the tags based on their indices, and exposes multiple functions to get contained tags based on their indices, and vice versa.

When creating a <mark style="color: lightgray; background-color: #191a18">TagCollection</mark>, you can pass a <mark style="color: lightgray; background-color: #191a18">[ITMPTagValidator](../api/TMPEffects.Tags.ITMPTagValidator.yml)</mark>, to guarantee you may only add specific tags to the collection.

For normal use cases, you won't be creating <mark style="color: lightgray; background-color: #191a18">TagCollections</mark> though; generally, you will use the <mark style="color: lightgray; background-color: #191a18">TagCollections</mark>
exposed by the TMPAnimator and TMPWriter to add or remove tags from script.

Be aware that if you add tags to a <mark style="color: lightgray; background-color: #191a18">TagCollection</mark>, if there is already a tag present with the exact same <mark style="color: lightgray; background-color: #191a18">StartIndex</mark> and <mark style="color: lightgray; background-color: #191a18">OrderAtIndex</mark>, that the <mark style="color: lightgray; background-color: #191a18">OrderAtIndex</mark> of the contained tags will be adjusted.