public class AlmUnit
{
    public float X;
    public float Y;
    public ushort TypeID;
    public ushort Face;
    public uint Flags;
    public uint Flags2;
    public int ServerID;
    public int Player;
    public int Sack;
    public int Angle;
    public short Health;
    public short HealthMax;
    public int ID;
    public int Group;

    public void LoadFromStream(BinaryReader br)
    {
        int xRaw = br.ReadInt32();
        int yRaw = br.ReadInt32();
        X = ((xRaw & 0x0000FF00) >> 8) + (float)(xRaw & 0x00FF) / 256; // 00 10 00 80 = 16.5
        Y = ((yRaw & 0x0000FF00) >> 8) + (float)(yRaw & 0x00FF) / 256;
        TypeID = br.ReadUInt16();
        Face = br.ReadUInt16();
        Flags = br.ReadUInt32();
        Flags2 = br.ReadUInt32();
        ServerID = br.ReadInt32();
        Player = br.ReadInt32();
        Sack = br.ReadInt32();
        Angle = br.ReadInt32();
        Health = br.ReadInt16();
        HealthMax = br.ReadInt16();
        ID = br.ReadInt32() & 0xFFFF;
        Group = br.ReadInt32();
    }
}
