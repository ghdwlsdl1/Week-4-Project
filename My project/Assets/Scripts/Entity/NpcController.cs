using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NpcType
{
    Guide,      // 조작법 안내 NPC
    Minigame    // 미니게임 실행 NPC
}

public class NpcController : MonoBehaviour
{
    public NpcType npcType; // 인스펙터에서 설정
}
