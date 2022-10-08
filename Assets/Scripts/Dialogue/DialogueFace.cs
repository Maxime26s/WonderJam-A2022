using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Face")]
public class DialogueFace : ScriptableObject
{
    public Sprite mouthClosed;
    public Sprite mouthOpened;
    public new string name;
    public Color nameColor;
}
