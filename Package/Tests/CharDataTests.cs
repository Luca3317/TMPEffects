using System.Collections;
using NUnit.Framework;
using TMPEffects.CharacterData;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using TMPEffects.Components;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPEffects.TextProcessing;
using TMPEffects.EffectCategories;
using TMPEffects.Modifiers;

public class CharDataTests
{
    private TMP_CharacterInfo characterInfo = new TMP_CharacterInfo()
    {
        ascender = 12,
        color = Color.red,
        index = 23,
        character = 'D',
        baseLine = 3,
        vertex_BL = new TMP_Vertex()
            { color = Color.black, position = Vector3.one * 10, uv = Vector3.one, uv2 = Vector3.one * 2.5f },
        vertex_TL = new TMP_Vertex()
            { color = Color.red, position = Vector3.one * 10, uv = Vector3.one, uv2 = Vector3.one * 2.5f },
        vertex_TR = new TMP_Vertex()
            { color = Color.green, position = Vector3.one * 5, uv = Vector3.one * 0.4f, uv2 = Vector3.one * 22.5f },
        vertex_BR = new TMP_Vertex()
            { color = Color.blue, position = Vector3.one * 2, uv = Vector3.one * 0.8f, uv2 = Vector3.one * 24.5f },
    };

    [Test]
    public void CreationTests()
    {
        CharData cData = new CharData(34, characterInfo, 2);

        Assert.AreEqual(12, cData.info.ascender);
        Assert.AreEqual((Color32)Color.red, cData.info.color);
        Assert.AreEqual(34, cData.info.index);
        Assert.AreEqual('D', cData.info.character);
        Assert.AreEqual(3, cData.info.baseLine);

        Assert.AreEqual(characterInfo.vertex_BL.position, cData.InitialMesh.BL_Position);
        Assert.AreEqual(characterInfo.vertex_BL.uv, (Vector2)cData.InitialMesh.BL_UV0);
        Assert.AreEqual(characterInfo.vertex_BL.uv2, (Vector2)cData.InitialMesh.BL_UV2);
        Assert.AreEqual(characterInfo.vertex_BL.color, cData.InitialMesh.BL_Color);

        Assert.AreEqual(characterInfo.vertex_BL.position, cData.mesh.BL_Position);
        Assert.AreEqual(characterInfo.vertex_BL.uv, (Vector2)cData.mesh.BL_UV0);
        Assert.AreEqual(characterInfo.vertex_BL.uv2, (Vector2)cData.mesh.BL_UV2);
        Assert.AreEqual(characterInfo.vertex_BL.color, cData.mesh.BL_Color);

        Assert.AreEqual(characterInfo.vertex_TL.position, cData.InitialMesh.TL_Position);
        Assert.AreEqual(characterInfo.vertex_TL.uv, (Vector2)cData.InitialMesh.TL_UV0);
        Assert.AreEqual(characterInfo.vertex_TL.uv2, (Vector2)cData.InitialMesh.TL_UV2);
        Assert.AreEqual(characterInfo.vertex_TL.color, cData.InitialMesh.TL_Color);

        Assert.AreEqual(characterInfo.vertex_TL.position, cData.mesh.TL_Position);
        Assert.AreEqual(characterInfo.vertex_TL.uv, (Vector2)cData.mesh.TL_UV0);
        Assert.AreEqual(characterInfo.vertex_TL.uv2, (Vector2)cData.mesh.TL_UV2);
        Assert.AreEqual(characterInfo.vertex_TL.color, cData.mesh.TL_Color);

        Assert.AreEqual(characterInfo.vertex_TR.position, cData.InitialMesh.TR_Position);
        Assert.AreEqual(characterInfo.vertex_TR.uv, (Vector2)cData.InitialMesh.TR_UV0);
        Assert.AreEqual(characterInfo.vertex_TR.uv2, (Vector2)cData.InitialMesh.TR_UV2);
        Assert.AreEqual(characterInfo.vertex_TR.color, cData.InitialMesh.TR_Color);

        Assert.AreEqual(characterInfo.vertex_TR.position, cData.mesh.TR_Position);
        Assert.AreEqual(characterInfo.vertex_TR.uv, (Vector2)cData.mesh.TR_UV0);
        Assert.AreEqual(characterInfo.vertex_TR.uv2, (Vector2)cData.mesh.TR_UV2);
        Assert.AreEqual(characterInfo.vertex_TR.color, cData.mesh.TR_Color);

        Assert.AreEqual(characterInfo.vertex_BR.position, cData.InitialMesh.BR_Position);
        Assert.AreEqual(characterInfo.vertex_BR.uv, (Vector2)cData.InitialMesh.BR_UV0);
        Assert.AreEqual(characterInfo.vertex_BR.uv2, (Vector2)cData.InitialMesh.BR_UV2);
        Assert.AreEqual(characterInfo.vertex_BR.color, cData.InitialMesh.BR_Color);

        Assert.AreEqual(characterInfo.vertex_BR.position, cData.mesh.BR_Position);
        Assert.AreEqual(characterInfo.vertex_BR.uv, (Vector2)cData.mesh.BR_UV0);
        Assert.AreEqual(characterInfo.vertex_BR.uv2, (Vector2)cData.mesh.BR_UV2);
        Assert.AreEqual(characterInfo.vertex_BR.color, cData.mesh.BR_Color);

        Vector3 center = Vector3.zero;
        center += (cData.mesh.BL_Position + cData.mesh.TL_Position + cData.mesh.TR_Position + cData.mesh.BR_Position);
        center /= 4f;

        Assert.AreEqual(Quaternion.identity, cData.InitialRotation);
        Assert.AreEqual(Vector3.one, cData.InitialScale);
        Assert.IsTrue(Vector3.Magnitude(center - cData.InitialPosition) < 0.001f);


        cData = new CharData(0, characterInfo, 2, new TMP_WordInfo());

        Assert.AreEqual(12, cData.info.ascender);
        Assert.AreEqual((Color32)Color.red, cData.info.color);
        Assert.AreEqual(0, cData.info.index);
        Assert.AreEqual('D', cData.info.character);
        Assert.AreEqual(3, cData.info.baseLine);

        Assert.AreEqual(characterInfo.vertex_BL.position, cData.InitialMesh.BL_Position);
        Assert.AreEqual(characterInfo.vertex_BL.uv, (Vector2)cData.InitialMesh.BL_UV0);
        Assert.AreEqual(characterInfo.vertex_BL.uv2, (Vector2)cData.InitialMesh.BL_UV2);
        Assert.AreEqual(characterInfo.vertex_BL.color, cData.InitialMesh.BL_Color);

        Assert.AreEqual(characterInfo.vertex_BL.position, cData.mesh.BL_Position);
        Assert.AreEqual(characterInfo.vertex_BL.uv, (Vector2)cData.mesh.BL_UV0);
        Assert.AreEqual(characterInfo.vertex_BL.uv2, (Vector2)cData.mesh.BL_UV2);
        Assert.AreEqual(characterInfo.vertex_BL.color, cData.mesh.BL_Color);

        Assert.AreEqual(characterInfo.vertex_TL.position, cData.InitialMesh.TL_Position);
        Assert.AreEqual(characterInfo.vertex_TL.uv, (Vector2)cData.InitialMesh.TL_UV0);
        Assert.AreEqual(characterInfo.vertex_TL.uv2, (Vector2)cData.InitialMesh.TL_UV2);
        Assert.AreEqual(characterInfo.vertex_TL.color, cData.InitialMesh.TL_Color);

        Assert.AreEqual(characterInfo.vertex_TL.position, cData.mesh.TL_Position);
        Assert.AreEqual(characterInfo.vertex_TL.uv, (Vector2)cData.mesh.TL_UV0);
        Assert.AreEqual(characterInfo.vertex_TL.uv2, (Vector2)cData.mesh.TL_UV2);
        Assert.AreEqual(characterInfo.vertex_TL.color, cData.mesh.TL_Color);

        Assert.AreEqual(characterInfo.vertex_TR.position, cData.InitialMesh.TR_Position);
        Assert.AreEqual(characterInfo.vertex_TR.uv, (Vector2)cData.InitialMesh.TR_UV0);
        Assert.AreEqual(characterInfo.vertex_TR.uv2, (Vector2)cData.InitialMesh.TR_UV2);
        Assert.AreEqual(characterInfo.vertex_TR.color, cData.InitialMesh.TR_Color);

        Assert.AreEqual(characterInfo.vertex_TR.position, cData.mesh.TR_Position);
        Assert.AreEqual(characterInfo.vertex_TR.uv, (Vector2)cData.mesh.TR_UV0);
        Assert.AreEqual(characterInfo.vertex_TR.uv2, (Vector2)cData.mesh.TR_UV2);
        Assert.AreEqual(characterInfo.vertex_TR.color, cData.mesh.TR_Color);

        Assert.AreEqual(characterInfo.vertex_BR.position, cData.InitialMesh.BR_Position);
        Assert.AreEqual(characterInfo.vertex_BR.uv, (Vector2)cData.InitialMesh.BR_UV0);
        Assert.AreEqual(characterInfo.vertex_BR.uv2, (Vector2)cData.InitialMesh.BR_UV2);
        Assert.AreEqual(characterInfo.vertex_BR.color, cData.InitialMesh.BR_Color);

        Assert.AreEqual(characterInfo.vertex_BR.position, cData.mesh.BR_Position);
        Assert.AreEqual(characterInfo.vertex_BR.uv, (Vector2)cData.mesh.BR_UV0);
        Assert.AreEqual(characterInfo.vertex_BR.uv2, (Vector2)cData.mesh.BR_UV2);
        Assert.AreEqual(characterInfo.vertex_BR.color, cData.mesh.BR_Color);

        center = Vector3.zero;
        center += (cData.mesh.BL_Position + cData.mesh.TL_Position + cData.mesh.TR_Position + cData.mesh.BR_Position);
        center /= 4f;

        Assert.AreEqual(Quaternion.identity, cData.InitialRotation);
        Assert.AreEqual(Vector3.one, cData.InitialScale);
        Assert.IsTrue(Vector3.Magnitude(center - cData.InitialPosition) < 0.001f);
    }

    [Test]
    public void PropertyTests()
    {
        CharData cData = new CharData(0, characterInfo, 2);

        cData.Position = new Vector3(8, 9, 10);
        cData.Scale = new Vector3(1, 2, 3);
        cData.AddRotation(new Vector3(90, 60, 90), new Vector3(4, 5, 6));

        Assert.AreEqual(new Vector3(8, 9, 10), cData.Position);
        Assert.AreEqual(new Vector3(8, 9, 10) - cData.InitialPosition, cData.PositionDelta);
        Assert.AreEqual(new Vector3(1, 2, 3), cData.Scale);
        Assert.AreEqual(1, cData.Rotations.Count);
        Assert.AreEqual(new Rotation(new Vector3(90, 60, 90), new Vector3(4, 5, 6)),
            cData.Rotations[0]);

        cData.Position += Vector3.one;
        cData.Scale += Vector3.one;
        cData.AddRotation(new Vector3(90, 60, 90) * 0.5f, new Vector3(4, 5, 6) * 9f);

        Assert.AreEqual(new Vector3(9, 10, 11), cData.Position);
        Assert.AreEqual(new Vector3(9, 10, 11) - cData.InitialPosition, cData.PositionDelta);
        Assert.AreEqual(new Vector3(2, 3, 4), cData.Scale);
        Assert.AreEqual(2, cData.Rotations.Count);
        Assert.AreEqual(new Rotation(new Vector3(90, 60, 90) * 0.5f, new Vector3(4, 5, 6) * 9f),
            cData.Rotations[1]);

        cData.PositionDelta = Vector3.one * 3;
        Assert.AreEqual(Vector3.one * 3, cData.PositionDelta);
        Assert.AreEqual(cData.InitialPosition + (Vector3.one * 3), cData.Position);

        cData.ClearPosition();
        cData.ClearRotations();
        cData.ClearScale();

        Assert.AreEqual(Vector3.zero, cData.PositionDelta);
        Assert.AreEqual(cData.InitialPosition, cData.Position);
        Assert.AreEqual(cData.InitialScale, cData.Scale);
        Assert.AreEqual(0, cData.Rotations.Count);
    }

    [Test]
    public void MeshPropertyTests()
    {
        CharData cData = new CharData(0, characterInfo, 2);

        cData.mesh.BL_Position = Vector3.one;
        cData.mesh.TL_Position = Vector3.one * 2;
        cData.mesh.TR_Position = Vector3.one * 3;
        cData.mesh.BR_Position = Vector3.one * 4;

        Assert.AreEqual(Vector3.one, cData.mesh.BL_Position);
        Assert.AreEqual(Vector3.one * 2, cData.mesh.TL_Position);
        Assert.AreEqual(Vector3.one * 3, cData.mesh.TR_Position);
        Assert.AreEqual(Vector3.one * 4, cData.mesh.BR_Position);

        cData.mesh.BL_Color = Color.red;
        cData.mesh.TL_Color = Color.black;
        cData.mesh.TR_Color = Color.green;
        cData.mesh.BR_Color = Color.magenta;

        Assert.AreEqual((Color32)Color.red, cData.mesh.BL_Color);
        Assert.AreEqual((Color32)Color.black, cData.mesh.TL_Color);
        Assert.AreEqual((Color32)Color.green, cData.mesh.TR_Color);
        Assert.AreEqual((Color32)Color.magenta, cData.mesh.BR_Color);

        cData.mesh.BL_Alpha = 5;
        cData.mesh.TL_Alpha = 15;
        cData.mesh.TR_Alpha = 115;
        cData.mesh.BR_Alpha = byte.MaxValue;

        Assert.AreEqual(5, cData.mesh.BL_Color.a);
        Assert.AreEqual(15, cData.mesh.TL_Color.a);
        Assert.AreEqual(115, cData.mesh.TR_Color.a);
        Assert.AreEqual(byte.MaxValue, cData.mesh.BR_Color.a);

        cData.mesh.BL_UV0 = new Vector2(0, 0);
        cData.mesh.TL_UV0 = new Vector2(0, 0);
        cData.mesh.TR_UV0 = new Vector2(0, 0);
        cData.mesh.BR_UV0 = new Vector2(0, 0);

        Assert.AreEqual(Vector3.zero, cData.mesh.BL_UV0);
        Assert.AreEqual(Vector3.zero, cData.mesh.TL_UV0);
        Assert.AreEqual(Vector3.zero, cData.mesh.TR_UV0);
        Assert.AreEqual(Vector3.zero, cData.mesh.BR_UV0);

        cData.mesh.BL_UV2 = new Vector2(0, 0);
        cData.mesh.TL_UV2 = new Vector2(0, 0);
        cData.mesh.TR_UV2 = new Vector2(0, 0);
        cData.mesh.BR_UV2 = new Vector2(0, 0);

        Assert.AreEqual(Vector3.zero, cData.mesh.BL_UV2);
        Assert.AreEqual(Vector3.zero, cData.mesh.TL_UV2);
        Assert.AreEqual(Vector3.zero, cData.mesh.TR_UV2);
        Assert.AreEqual(Vector3.zero, cData.mesh.BR_UV2);
    }
}