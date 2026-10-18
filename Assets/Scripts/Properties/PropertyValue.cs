using System;

[Serializable]
public struct PropertyValue {

    public float NumberValue;
    //public string StringValue;
    public bool BoolValue;

    public static PropertyValue Create(float value) {
        return new() {NumberValue = value};
    }

    public static PropertyValue Create(Property prop, string value) {
        PropertyValue propertyValue = new PropertyValue();
        for (int i = 0; i < prop.validStrings.Count; ++i) {
            if (value == prop.validStrings[i]) {
                propertyValue.NumberValue = i;
                return propertyValue;
            }
        }
        throw new ArgumentException($"Property {prop.propertyName} has been set to an invalid string: {value}");
    }

    public static PropertyValue Create(bool value) {
        return new() {BoolValue = value};
    }
}