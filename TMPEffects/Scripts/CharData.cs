using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct CharData
{
    #region CharacterInfo

    // TODO These structs are pretty big;
    // Make just hold initial mesh and for current mesh just indeces?
    public VertexData initialMesh;
    public VertexData currentMesh;

    public readonly int index;
    public readonly int wordFirstIndex;
    public readonly int wordLen;

    public readonly bool isVisible;
    // TODO Probably need to reset this one every time as well
    // Also large; maybe make characterdata a class and only keep some fields of info
    // CharData passed as reference though so maybe size not an issue
    //public TMP_CharacterInfo info;

    #endregion

    // This data would be made redundant if passing with Span
    public int segmentIndex;
    public int segmentLength;

    public bool hidden;

    public CharData(TMP_CharacterInfo characterInfo, TMP_WordInfo wordInfo) : this(characterInfo)
    {
        this.wordFirstIndex = wordInfo.firstCharacterIndex;
        this.wordLen = wordInfo.characterCount;
    }

    public CharData(TMP_CharacterInfo characterInfo)
    {
        VertexData vData = new VertexData(characterInfo);
        initialMesh = vData;
        currentMesh = vData;

        index = characterInfo.index;
        isVisible = characterInfo.isVisible;
        this.wordFirstIndex = -1;
        this.wordLen = -1;

        segmentIndex = -1;
        segmentLength = -1;

        hidden = false;
    }


    public struct VertexData
    {
        public TMP_Vertex vertex_TL;
        public TMP_Vertex vertex_TR;
        public TMP_Vertex vertex_BR;
        public TMP_Vertex vertex_BL;

        public TMP_Vertex this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return vertex_BL;
                    case 1: return vertex_TL;
                    case 2: return vertex_TR;
                    case 3: return vertex_BR;
                    default: throw new System.ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (i)
                {
                    case 0: vertex_BL = value; break;
                    case 1: vertex_TL = value; break;
                    case 2: vertex_TR = value; break;
                    case 3: vertex_BR = value; break;
                    default: throw new System.ArgumentOutOfRangeException();
                }
            }
        }

        public VertexData(TMP_Vertex bl, TMP_Vertex tl, TMP_Vertex tr, TMP_Vertex br)
        {
            this.vertex_BL = bl;
            this.vertex_TL = tl;
            this.vertex_TR = tr;
            this.vertex_BR = br;
        }

        public VertexData(TMP_CharacterInfo info)
        {
            this.vertex_BL = info.vertex_BL;
            this.vertex_TL = info.vertex_TL;
            this.vertex_TR = info.vertex_TR;
            this.vertex_BR = info.vertex_BR;
        }

        public Vector3 GetPosition(int i)
        {
            switch (i)
            {
                case 0: return vertex_BL.position;
                case 1: return vertex_TL.position;
                case 2: return vertex_TR.position;
                case 3: return vertex_BR.position;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public void SetPosition(int i, Vector3 value)
        {
            switch (i)
            {
                case 0: vertex_BL.position = value; break;
                case 1: vertex_TL.position = value; break;
                case 2: vertex_TR.position = value; break;
                case 3: vertex_BR.position = value; break;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public Color32 GetColor(int i)
        {
            switch (i)
            {
                case 0: return vertex_BL.color;
                case 1: return vertex_TL.color;
                case 2: return vertex_TR.color;
                case 3: return vertex_BR.color;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }

        public void SetColor(int i, Color32 value)
        {
            switch (i)
            {
                case 0: vertex_BL.color = value; break;
                case 1: vertex_TL.color = value; break;
                case 2: vertex_TR.color = value; break;
                case 3: vertex_BR.color = value; break;
                default: throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}
