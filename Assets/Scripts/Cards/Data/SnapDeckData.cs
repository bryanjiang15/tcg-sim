using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SnapDeckData
{
    public string deckArtPath;
    public List<int> CardIds;
}

public class SnapDeckLibraryData
{
    public List<SnapDeckData> decks;
}
