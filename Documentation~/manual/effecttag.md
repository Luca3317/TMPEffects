<link rel="stylesheet" type="text/css" href="../styles.css">

# TMPEffectTag
<mark class="markstyle">TMPEffectTag</mark>, together with <mark class="markstyle">TMPEffectTagIndices</mark>, is the data structure used to represent a tag in the code, for example animation tags or command tags.  
The API docs can be found [here](../api/TMPEffects.Tags.EffectTag.yml).

## Properties
The <mark class="markstyle">TMPEffectTag</mark> consists of the following properties:

- Name : <mark class="markstyle">string</mark>
- Prefix : <mark class="markstyle">char</mark>
- Parameters : <mark class="markstyle">ReadOnlyDictionary&lt;string, string&gt;</mark>

Example: the show tag <mark class="markstyle">&lt;+fade anc=zero dur=0.55&gt;</mark> would be:

- Name : "fade"
- Prefix : '+'
- Parameters : {{ "anc", "zero" }, { "dur", "0.55" }}

## Indices
The <mark class="markstyle">TMPEffectTagIndices</mark> struct consists of:

- StartIndex : <mark class="markstyle">int</mark>
- EndIndex : <mark class="markstyle">int</mark>
- OrderAtIndex : <mark class="markstyle">int</mark>

The indices are a half open interval; meaning a tag with a StartIndex of 5 and an EndIndex of 12 will "contain" the indices 5, 6, 7, 8, 9, 10, 11 and 12.

The OrderAtIndex is used to maintain an order if there are multiple tags at the same index. Generally speaking, the tags' OrderAtIndex won't be sequential (i.e. 1, 2, 3, and so on), but may skip
around. You will notice this if you iterate, for example, over the <mark class="markstyle">BasicTags</mark> property of a TMPAnimator.
The only invariant OrderAtIndex it guaranteed to follow is that they are sorted from smallest to largest.