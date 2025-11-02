
using System.Collections;
using System.Collections.Generic;

public class StatTypeModal {
    public int StatTypeId { get; set; }

    public string Name { get; set; }

    public StatValueType StatValueType { get; set; }
}

public class ValueStatTypeModal : StatTypeModal {
    public int MaxValue { get; set; }

    public int MinValue { get; set; }
}

public class LevelStatTypeModal : StatTypeModal {
    public IEnumerable<string> Levels { get; set; }
}