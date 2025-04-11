using AllodsParser;
using SimpleTiled;

public class AlmFileLoader : BaseFileLoader
{
    protected override BaseFile LoadInternal(string relativeFilePath, MemoryStream ms, BinaryReader br)
    {
        // first, read in the global header
        uint alm_signature = br.ReadUInt32();
        uint alm_headersize = br.ReadUInt32();
        br.BaseStream.Position += 4;  // uint alm_offsetplayers = msb.ReadUInt32();
        uint alm_sectioncount = br.ReadUInt32();
        br.BaseStream.Position += 4; // uint alm_version = msb.ReadUInt32();

        if ((alm_signature != 0x0052374D) ||
            (alm_headersize != 0x14) ||
            (alm_sectioncount < 3))
        {
            Console.Error.WriteLine($"Invalid signature of allods map {relativeFilePath}.");
            return new EmptyFile();
        }

        var alm = new AlmFile();
        
        bool DataLoaded = false;
        bool TilesLoaded = false;
        bool HeightsLoaded = false;

        for (uint i = 0; i < alm_sectioncount; i++)
        {
            br.BaseStream.Position += 4; // uint sec_junk1 = msb.ReadUInt32();
            br.BaseStream.Position += 4; // uint sec_headersize = msb.ReadUInt32();
            uint sec_size = br.ReadUInt32();
            uint sec_id = br.ReadUInt32();
            br.BaseStream.Position += 4; // uint sec_junk2 = msb.ReadUInt32();

            switch (sec_id)
            {
                case 0: // data
                    alm.Data.LoadFromStream(br);
                    DataLoaded = true;
                    break;
                case 1: // tiles
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for tiles");
                        return new EmptyFile();
                    }

                    alm.Tiles = new ushort[alm.Data.Width * alm.Data.Height];
                    for (uint j = 0; j < alm.Data.Width * alm.Data.Height; j++)
                        alm.Tiles[j] = br.ReadUInt16();
                    TilesLoaded = true;
                    break;
                case 2: // heights
                    if (!TilesLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !TilesLoaded for heights");
                        return new EmptyFile();
                    }

                    alm.Heights = new sbyte[alm.Data.Width * alm.Data.Height];
                    for (uint j = 0; j < alm.Data.Width * alm.Data.Height; j++)
                        alm.Heights[j] = br.ReadSByte();
                    HeightsLoaded = true;
                    break;
                case 3: // objects (obstacles)
                    if (!HeightsLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !HeightsLoaded for obstacles");
                        return new EmptyFile();
                    }

                    alm.Objects = new byte[alm.Data.Width * alm.Data.Height];
                    for (uint j = 0; j < alm.Data.Width * alm.Data.Height; j++)
                        alm.Objects[j] = br.ReadByte();
                    break;
                case 4: // structures
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for structures");
                        return new EmptyFile();
                    }

                    alm.Structures = new AlmFile.AlmStructure[alm.Data.CountStructures];
                    for (uint j = 0; j < alm.Data.CountStructures; j++)
                    {
                        alm.Structures[j] = new AlmFile.AlmStructure();
                        alm.Structures[j].LoadFromStream(br);
                    }
                    break;
                case 5: // players
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for players");
                        return new EmptyFile();
                    }

                    alm.Players = new AlmFile.AlmPlayer[alm.Data.CountPlayers];
                    for (uint j = 0; j < alm.Data.CountPlayers; j++)
                    {
                        alm.Players[j] = new AlmFile.AlmPlayer();
                        alm.Players[j].LoadFromStream(br);
                    }
                    break;
                case 6: // units
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for units");
                        return new EmptyFile();
                    }

                    alm.Units = new AlmFile.AlmUnit[alm.Data.CountUnits];
                    for (uint j = 0; j < alm.Data.CountUnits; j++)
                    {
                        alm.Units[j] = new AlmFile.AlmUnit();
                        alm.Units[j].LoadFromStream(br);
                    }
                    break;
                case 7: // Logic
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for logic");
                        return new EmptyFile();
                    }

                    alm.Logic = new AlmFile.AlmLogic();
                    alm.Logic.LoadFromStream(br);
                    break;
                case 8: // Sack
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for sacks");
                        return new EmptyFile();
                    }

                    alm.Sacks = new AlmFile.AlmSack[alm.Data.CountSacks];
                    for (int j = 0; j < alm.Data.CountSacks; j++)
                    {
                        alm.Sacks[j] = new AlmFile.AlmSack();
                        alm.Sacks[j].LoadFromStream(br);
                    }
                    break;
                case 9: // Effects
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for effects");
                        return new EmptyFile();
                    }

                    var numberOfEffects = br.ReadUInt32();
                    alm.Effects = new AlmFile.AlmEffect[numberOfEffects];
                    for (int j = 0; j < numberOfEffects; j++)
                    {
                        alm.Effects[j] = new AlmFile.AlmEffect();
                        alm.Effects[j].LoadFromStream(br);
                    }
                    break;
                case 10: // Groups 
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for groups");
                        return new EmptyFile();
                    }

                    alm.Groups = new AlmFile.AlmGroup[alm.Data.CountGroups];
                    for (int j = 0; j < alm.Data.CountGroups; j++)
                    {
                        alm.Groups[j] = new AlmFile.AlmGroup();
                        alm.Groups[j].LoadFromStream(br);
                    }
                    break;
                case 11: // Options
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for options");
                        return new EmptyFile();
                    }

                    alm.Inns = new AlmFile.AlmInnInfo[alm.Data.CountInns];
                    for (int j = 0; j < alm.Data.CountInns; j++)
                    {
                        alm.Inns[j] = new AlmFile.AlmInnInfo();
                        alm.Inns[j].LoadFromStream(br);
                    }

                    alm.Shops = new AlmFile.AlmShop[alm.Data.CountShops];
                    for (int j = 0; j < alm.Data.CountShops; j++)
                    {
                        alm.Shops[j] = new AlmFile.AlmShop();
                        alm.Shops[j].LoadFromStream(br);
                    }

                    alm.Pointers = new AlmFile.AlmOptionPointer[alm.Data.CountPointers];
                    for (int j = 0; j < alm.Data.CountPointers; j++)
                    {
                        alm.Pointers[j] = new AlmFile.AlmOptionPointer();
                        alm.Pointers[j].LoadFromStream(br);
                    }
                    break;
                case 12: // Music
                    alm.Music = new AlmFile.AlmMusic[alm.Data.CountMusic + 1];
                    for (int j = 0; j < alm.Music.Length; j++)
                    {
                        alm.Music[j] = new AlmFile.AlmMusic();
                        alm.Music[j].LoadFromStream(br);
                    }
                    break;
                default:
                    ms.Position += sec_size;
                    break;
            }
        }
    
        return alm;
    }
}
