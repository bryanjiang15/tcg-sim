using System.Collections.Generic;

/// <summary>
/// SnapCardTypeModal is a modal for a card type.
/// It contains the card type name and the stats that the card type has.
/// This represent the infomation needed for a card of this type, for example, a spell card, monster card, energy card, etc.
/// </summary>
public class SnapCardTypeModal {
    public int SnapCardTypeId { get; set; }

    public string CardTypeName { get; set; }

    public List<StatTypeModal> Stats { get; set; }
    
}