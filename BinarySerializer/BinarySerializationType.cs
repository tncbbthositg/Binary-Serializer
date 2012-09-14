namespace com.AutopilotLlc.BinarySerializer
{
    internal enum BinarySerializationType
    {
        // built in types
        // bool is left out of this list as I'm saving a little space by encoding the two values
        Byte,
        Char,
        Decimal,
        Double,
        Int,
        Long,
        SByte,
        Short,
        String,
        UInt,
        ULong,
        UShort,

        // other types
        DateTime,
        TimeSpan,

        // special types
        Collection,
        Dictionary,
        Null,
        Object,
        True,
        False
    }
}
