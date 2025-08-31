using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SnapDeckData
{
    public int deckId;
    public string deckName;
    public DateTime dateCreated;
    public DateTime dateUpdated;
    public string deckArtPath;
    public List<int> CardIds;
}

public class SnapDeckLibraryData
{
    public List<SnapDeckData> decks;
}
