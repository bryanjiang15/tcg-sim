using System;
using System.Collections.Generic;
using UnityEngine;

public struct CreateCardData
{
    public string cardName;
    public List<Tuple<BuffType, int>> buffs;
}

public class CreateCardSystem : Singleton<CreateCardSystem> {
    
}