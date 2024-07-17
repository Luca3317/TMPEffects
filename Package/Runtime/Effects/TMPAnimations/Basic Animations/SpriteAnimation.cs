using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.TextProcessing;
using TMPro;
using UnityEngine;
using static TMPEffects.ParameterUtility;


namespace TMPEffects.TMPAnimations.Animations
{
    internal class SpriteAnimation : ITMPAnimation
    {
        public void Animate(CharData cData, IAnimationContext context)
        {
            // Return if is not a sprite
            if (cData.info.elementType != TMP_TextElementType.Sprite) return;

            // Return if sprite was truncated or replaced by the Ellipsis character.
            if (cData.info.character == 0x03 || cData.info.character == 0x2026) return;

            Data d = context.CustomData as Data;
            Data2 d2;

            if (!d.datas.ContainsKey(cData.info.index))
            {
                d2 = new Data2();
                if (d.framerate < 0)
                {
                    d2.currentFrame = d.end;
                }
                else
                {
                    d2.currentFrame = d.start;
                }
                if (d.end > cData.info.spriteAsset.spriteCharacterTable.Count)
                    d.end = cData.info.spriteAsset.spriteCharacterTable.Count - 1;

                d2.baseSpriteScale = cData.info.spriteAsset.spriteCharacterTable[d.start].scale * cData.info.spriteAsset.spriteCharacterTable[d.start].glyph.scale;
                d2.targetTime = 0f;
                d.datas.Add(cData.info.index, d2);
            }
            else
            {
                d2 = d.datas[cData.info.index];
            }
            if (d.iterations >= 0 && d.iterations <= d2.finishedIterations)
            {
                TMP_SpriteAsset spriteAsset = cData.info.spriteAsset;

                d2.targetTime = 1f / Mathf.Abs(d.framerate) + context.AnimatorContext.PassedTime;

                // Get a reference to the current sprite
                TMP_SpriteCharacter spriteCharacter = cData.info.spriteAsset.spriteCharacterTable[d.framerate > 0 ? d.end : d.start];

                // Update the vertices for the new sprite
                Vector2 origin = new Vector2(cData.info.origin, cData.info.baseLine);

                float spriteScale = cData.info.referenceScale / d2.baseSpriteScale * spriteCharacter.scale * spriteCharacter.glyph.scale;

                Vector3 bl = new Vector3(origin.x + spriteCharacter.glyph.metrics.horizontalBearingX * spriteScale, origin.y + (spriteCharacter.glyph.metrics.horizontalBearingY - spriteCharacter.glyph.metrics.height) * spriteScale);
                Vector3 tl = new Vector3(bl.x, origin.y + spriteCharacter.glyph.metrics.horizontalBearingY * spriteScale);
                Vector3 tr = new Vector3(origin.x + (spriteCharacter.glyph.metrics.horizontalBearingX + spriteCharacter.glyph.metrics.width) * spriteScale, tl.y);
                Vector3 br = new Vector3(tr.x, bl.y);

                d2.vert_0 = bl;
                d2.vert_1 = tl;
                d2.vert_2 = tr;
                d2.vert_3 = br;

                // Update the UV to point to the new sprite
                Vector2 uv0 = new Vector2((float)spriteCharacter.glyph.glyphRect.x / spriteAsset.spriteSheet.width, (float)spriteCharacter.glyph.glyphRect.y / spriteAsset.spriteSheet.height);
                Vector2 uv1 = new Vector2(uv0.x, (float)(spriteCharacter.glyph.glyphRect.y + spriteCharacter.glyph.glyphRect.height) / spriteAsset.spriteSheet.height);
                Vector2 uv2 = new Vector2((float)(spriteCharacter.glyph.glyphRect.x + spriteCharacter.glyph.glyphRect.width) / spriteAsset.spriteSheet.width, uv1.y);
                Vector2 uv3 = new Vector2(uv2.x, uv0.y);

                d2.uv0_0 = uv0;
                d2.uv0_1 = uv1;
                d2.uv0_2 = uv2;
                d2.uv0_3 = uv3;
            }
            else if (context.AnimatorContext.PassedTime >= d2.targetTime)
            {
                TMP_SpriteAsset spriteAsset = cData.info.spriteAsset;

                d2.targetTime = 1f / Mathf.Abs(d.framerate) + context.AnimatorContext.PassedTime;

                // Get a reference to the current sprite
                TMP_SpriteCharacter spriteCharacter;

                switch (d.evaluation)
                {
                    case "pingpong":
                    case "pp":

                        if (d.framerate > 0)
                        {
                            var v = d2.currentFrame % (d.end * 2);
                            if (v > d.end) v = d.end - (v - d.end);

                            spriteCharacter = spriteAsset.spriteCharacterTable[v];
                            d2.currentFrame += 1;

                            if (Mathf.Abs(d2.currentFrame) == d.end + (d.end - d.start))
                            {
                                d2.currentFrame = d.start;
                                d2.finishedIterations++;
                            }
                        }
                        else
                        {
                            var v = d2.currentFrame % (d.start * 2);
                            if (v < d.start) v = d.start - (v - d.start);

                            spriteCharacter = spriteAsset.spriteCharacterTable[v];
                            d2.currentFrame -= 1;

                            if (Mathf.Abs(d2.currentFrame) == d.start - (d.end - d.start))
                            {
                                d2.currentFrame = d.end;
                                d2.finishedIterations++;
                            }
                        }
                        break;

                    case "loop":
                    case "lp":
                        spriteCharacter = spriteAsset.spriteCharacterTable[d2.currentFrame];
                        if (d.framerate > 0)
                        {
                            if (d2.currentFrame < d.end)
                                d2.currentFrame += 1;
                            else
                            {
                                d2.currentFrame = d.start;
                                d2.finishedIterations++;
                            }
                        }
                        else
                        {
                            if (d2.currentFrame > d.start)
                                d2.currentFrame -= 1;
                            else
                            {
                                d2.currentFrame = d.end;
                                d2.finishedIterations++;
                            }
                        }
                        break;

                    default: throw new System.Exception("Invalid evaluation state");
                }


                // Update the vertices for the new sprite
                Vector2 origin = new Vector2(cData.info.origin, cData.info.baseLine);

                float spriteScale = cData.info.referenceScale / d2.baseSpriteScale * spriteCharacter.scale * spriteCharacter.glyph.scale;

                Vector3 bl = new Vector3(origin.x + spriteCharacter.glyph.metrics.horizontalBearingX * spriteScale, origin.y + (spriteCharacter.glyph.metrics.horizontalBearingY - spriteCharacter.glyph.metrics.height) * spriteScale);
                Vector3 tl = new Vector3(bl.x, origin.y + spriteCharacter.glyph.metrics.horizontalBearingY * spriteScale);
                Vector3 tr = new Vector3(origin.x + (spriteCharacter.glyph.metrics.horizontalBearingX + spriteCharacter.glyph.metrics.width) * spriteScale, tl.y);
                Vector3 br = new Vector3(tr.x, bl.y);

                d2.vert_0 = bl;
                d2.vert_1 = tl;
                d2.vert_2 = tr;
                d2.vert_3 = br;

                // Update the UV to point to the new sprite
                Vector2 uv0 = new Vector2((float)spriteCharacter.glyph.glyphRect.x / spriteAsset.spriteSheet.width, (float)spriteCharacter.glyph.glyphRect.y / spriteAsset.spriteSheet.height);
                Vector2 uv1 = new Vector2(uv0.x, (float)(spriteCharacter.glyph.glyphRect.y + spriteCharacter.glyph.glyphRect.height) / spriteAsset.spriteSheet.height);
                Vector2 uv2 = new Vector2((float)(spriteCharacter.glyph.glyphRect.x + spriteCharacter.glyph.glyphRect.width) / spriteAsset.spriteSheet.width, uv1.y);
                Vector2 uv3 = new Vector2(uv2.x, uv0.y);

                d2.uv0_0 = uv0;
                d2.uv0_1 = uv1;
                d2.uv0_2 = uv2;
                d2.uv0_3 = uv3;
            }

            cData.mesh.SetPosition(0, d2.vert_0);
            cData.mesh.SetPosition(1, d2.vert_1);
            cData.mesh.SetPosition(2, d2.vert_2);
            cData.mesh.SetPosition(3, d2.vert_3);

            cData.mesh.SetUV0(0, d2.uv0_0);
            cData.mesh.SetUV0(1, d2.uv0_1);
            cData.mesh.SetUV0(2, d2.uv0_2);
            cData.mesh.SetUV0(3, d2.uv0_3);
        }

        public object GetNewCustomData()
        {
            return new Data() { datas = new Dictionary<int, Data2>(), start = 0, end = 0, framerate = 0, evaluation = "loop", iterations = -1 };
        }

        public void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            Data d = customData as Data;

            var split = parameters["anim"].Split(',');
            if (ParsingUtility.StringToInt(split[0], out int result))
            {
                d.start = result;
            }
            if (ParsingUtility.StringToInt(split[1], out result))
            {
                d.end = result;
            }
            if (ParsingUtility.StringToInt(split[2], out result))
            {
                d.framerate = result;
            }

            if (TryGetIntParameter(out int value, parameters, "iterations", "iter")) d.iterations = value;
            if (TryGetDefinedParameter(out string v, parameters, "evaluation", "eval"))
            {
                switch (parameters[v])
                {
                    case "pingpong":
                    case "pp":
                    case "loop":
                    case "lp": d.evaluation = parameters[v]; break;
                }
            }
        }

        public bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return false;

            if (!parameters.ContainsKey("anim")) return false;
            if (parameters["anim"].Split(',').Length != 3) return false;
            return true;
        }

        private class Data
        {
            public int start;
            public int end;
            public int framerate;
            public string evaluation;
            public int iterations;

            public Dictionary<int, Data2> datas;
        }

        private class Data2
        {
            public int currentFrame;
            public float baseSpriteScale;
            public float targetTime;

            public int finishedIterations;

            public Vector2 uv0_0;
            public Vector2 uv0_1;
            public Vector2 uv0_2;
            public Vector2 uv0_3;

            public Vector3 vert_0;
            public Vector3 vert_1;
            public Vector3 vert_2;
            public Vector3 vert_3;

            public bool init;
        }
    }
}