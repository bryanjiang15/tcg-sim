
public class StatType {
    public int StatTypeId { get; set; }

    public string Name { get; set; }

    public StatValueType StatValueType { get; set; }
}

public class ValueStatType : StatType {
    public int MaxValue { get; set; }

    public int MinValue { get; set; }
}