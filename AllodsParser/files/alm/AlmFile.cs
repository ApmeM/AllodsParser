﻿using AllodsParser;
using SimpleTiled;

public class AlmFile : BaseFile<AlmMap, TmxMap>
{
    protected override AlmMap? LoadInternal(MemoryStream ms, BinaryReader br)
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
            return null;
        }

        var alm = new AlmMap();

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
                        return null;
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
                        return null;
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
                        return null;
                    }

                    alm.Objects = new byte[alm.Data.Width * alm.Data.Height];
                    for (uint j = 0; j < alm.Data.Width * alm.Data.Height; j++)
                        alm.Objects[j] = br.ReadByte();
                    break;
                case 4: // structures
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for structures");
                        return null;
                    }

                    alm.Structures = new AlmStructure[alm.Data.CountStructures];
                    for (uint j = 0; j < alm.Data.CountStructures; j++)
                    {
                        alm.Structures[j] = new AlmStructure();
                        alm.Structures[j].LoadFromStream(br);
                    }
                    break;
                case 5: // players
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for players");
                        return null;
                    }

                    alm.Players = new AlmPlayer[alm.Data.CountPlayers];
                    for (uint j = 0; j < alm.Data.CountPlayers; j++)
                    {
                        alm.Players[j] = new AlmPlayer();
                        alm.Players[j].LoadFromStream(br);
                    }
                    break;
                case 6: // units
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for units");
                        return null;
                    }

                    alm.Units = new AlmUnit[alm.Data.CountUnits];
                    for (uint j = 0; j < alm.Data.CountUnits; j++)
                    {
                        alm.Units[j] = new AlmUnit();
                        alm.Units[j].LoadFromStream(br);
                    }
                    break;
                case 7: // Logic
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for logic");
                        return null;
                    }

                    alm.Logic = new AlmLogic();
                    alm.Logic.LoadFromStream(br);
                    break;
                case 8: // Sack
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for sacks");
                        return null;
                    }

                    alm.Sacks = new AlmSack[alm.Data.CountSacks];
                    for (int j = 0; j < alm.Data.CountSacks; j++)
                    {
                        alm.Sacks[j] = new AlmSack();
                        alm.Sacks[j].LoadFromStream(br);
                    }
                    break;
                case 9: // Effects
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for effects");
                        return null;
                    }

                    var numberOfEffects = br.ReadUInt32();
                    alm.Effects = new AlmEffect[numberOfEffects];
                    for (int j = 0; j < numberOfEffects; j++)
                    {
                        alm.Effects[j] = new AlmEffect();
                        alm.Effects[j].LoadFromStream(br);
                    }
                    break;
                case 10: // Groups 
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for groups");
                        return null;
                    }

                    alm.Groups = new AlmGroup[alm.Data.CountGroups];
                    for (int j = 0; j < alm.Data.CountGroups; j++)
                    {
                        alm.Groups[j] = new AlmGroup();
                        alm.Groups[j].LoadFromStream(br);
                    }
                    break;
                case 11: // Options
                    if (!DataLoaded)
                    {
                        Console.WriteLine($"Invalid structure of allods map {relativeFilePath}: !DataLoaded for options");
                        return null;
                    }

                    alm.Inns = new AlmInnInfo[alm.Data.CountInns];
                    for (int j = 0; j < alm.Data.CountInns; j++)
                    {
                        alm.Inns[j] = new AlmInnInfo();
                        alm.Inns[j].LoadFromStream(br);
                    }

                    alm.Shops = new AlmShop[alm.Data.CountShops];
                    for (int j = 0; j < alm.Data.CountShops; j++)
                    {
                        alm.Shops[j] = new AlmShop();
                        alm.Shops[j].LoadFromStream(br);
                    }

                    alm.Pointers = new AlmOptionPointer[alm.Data.CountPointers];
                    for (int j = 0; j < alm.Data.CountPointers; j++)
                    {
                        alm.Pointers[j] = new AlmOptionPointer();
                        alm.Pointers[j].LoadFromStream(br);
                    }
                    break;
                case 12: // Music
                    alm.Music = new AlmMusic[alm.Data.CountMusic + 1];
                    for (int j = 0; j < alm.Music.Length; j++)
                    {
                        alm.Music[j] = new AlmMusic();
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

    protected override TmxMap? ConvertInternal(AlmMap toConvert)
    {
        var map = new TmxMap
        {
            Orientation = TmxOrientation.Orthogonal,
            RenderOrder = TmxRenderOrder.RightDown,
            Width = (int)toConvert.Data.Width,
            Height = (int)toConvert.Data.Height,
            TileWidth = 32,
            TileHeight = 32,
            TileSets = new List<TmxTileSet>(),
            Layers = new List<TmxLayer>(),
            ObjectGroups = new List<TmxObjectGroup>(),
            Properties = new List<TmxProperty>()
        };

        map.TileSets = new List<TmxTileSet>();

        for (int i = 0; i < 52; i++)
        {
            int t_c = ((i & 0xF0) >> 4) + 1;
            int t_d = (i & 0x0F);
            string t_fn = string.Format("../graphics/terrain/tile{0}-{1}.bmp", t_c, t_d.ToString().PadLeft(2, '0'));
            map.TileSets.Add(new TmxTileSet
            {
                Name = Path.GetFileName(t_fn).Replace("-", ""),
                TileWidth = 32,
                TileHeight = 32,
                Image = new TmxImage
                {
                    Source = t_fn
                },
                FirstGid = (i + 1) * 100
            });
        }

        map.Layers.Add(new TmxTileLayer
        {
            Name = "map",
            Width = (int)toConvert.Data.Width,
            Height = (int)toConvert.Data.Height,
            Visible = true,
            Data = new TmxData
            {
                Encoding = "csv",
                Tiles = toConvert.Tiles.Select(a =>
                {

                    ushort tile = a;
                    int tilenum = (tile & 0xFF0) >> 4; // base rect
                    int tilein = tile & 0x00F; // number of picture inside rect

                    if (tilenum >= 0x20 && tilenum <= 0x2F)
                    {
                        tilenum -= 0x20;
                        int tilewi = tilenum / 4;
                        int tilew = tilenum % 4;
                        int waflocal = tilewi;
                        tilenum = 0x20 + (4 * waflocal) + tilew;
                    }

                    return new TmxDataTile
                    {
                        Gid = (uint)((tilenum + 1) * 100 + tilein)
                    };
                }).ToList()
            }
        });

        // map.ObjectGroups.Add(new TmxObjectGroup
        // {
        //     Name = "Structures",
        //     Visible = true,
        //     Objects = toConvert.Structures.Select(a => new TmxObject
        //     {
        //         X = a.X,
        //         Y = a.Y,
        //         Width = a.Width,
        //         Height = a.Height,
        //         Gid = (uint)a.TypeID
        //     }).ToList()
        // });

        return map;
    }

    protected override void SaveInternal(string outputFileName, TmxMap toSave)
    {
        using (var f = File.OpenWrite(outputFileName.Replace(".alm", ".tmx")))
        {
            TiledHelper.Write(toSave, f);
        }
    }
}
