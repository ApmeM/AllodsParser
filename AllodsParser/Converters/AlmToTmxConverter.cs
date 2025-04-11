using SimpleTiled;
using SixLabors.ImageSharp;

namespace AllodsParser
{
    public class AlmToTmxConverter : BaseFileConverter
    {
        public override void Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<AlmFile>()
                .ToList();

            Console.WriteLine($"{this.GetType()} converts {oldFiles.Count} files");

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));
        }

        private IEnumerable<TmxFile> ConvertFile(AlmFile toConvert, List<BaseFile> files)
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

            var structures = files
                .OfType<RegStructureFile>()
                .Single()
                .Structures;

            foreach (var structure in structures)
            {
                map.TileSets.Add(new TmxTileSet
                {
                    Name = structure.File,
                    TileWidth = structure.TileWidth * 32,
                    TileHeight = structure.FullHeight * 32,
                    Image = new TmxImage
                    {
                        Source = $"../graphics/structures/{structure.File.ToLower()}.0.0.png"
                    },
                    FirstGid = 5500 + structure.Id

                });
            }

            map.ObjectGroups.Add(new TmxObjectGroup
            {
                Name = "Structures",
                Visible = true,
                Objects = toConvert.Structures.Select(a => new TmxObject
                {
                    X = a.X * 32 - 16,
                    Y = a.Y * 32 - 16 + a.Height,
                    Width = a.Width,
                    Height = a.Height,
                    Gid = 5500 + (uint)a.TypeID
                }).ToList()
            });

            var units = files
                .OfType<RegUnitsFile>()
                .Single()
                .Units
                .Where(a => a.Id >= 0)
                .ToList();

            foreach (var unit in units)
            {
                map.TileSets.Add(new TmxTileSet
                {
                    Name = unit.File,
                    TileWidth = unit.Width,
                    TileHeight = unit.Height,
                    Image = new TmxImage
                    {
                        Source = $"../graphics/units/{unit.File.ToLower()}.0.0.png"
                    },
                    FirstGid = 6000 + unit.Id

                });
            }

            map.ObjectGroups.Add(new TmxObjectGroup
            {
                Name = "Units",
                Visible = true,
                Objects = toConvert.Units.Select(a => new TmxObject
                {
                    X = a.X * 32 - 16,
                    Y = a.Y * 32 - 16 + units.First(b => b.Id == a.TypeID).Height,
                    Width = units.First(b => b.Id == a.TypeID).Width,
                    Height = units.First(b => b.Id == a.TypeID).Height,
                    Gid = 6000 + (uint)a.TypeID
                }).ToList()
            });

            var objects = files
                .OfType<RegObjectsFile>()
                .Single()
                .Objects
                .Where(a => a.Id >= 0)
                .ToList();

            foreach (var obj in objects)
            {
                map.TileSets.Add(new TmxTileSet
                {
                    Name = obj.File,
                    TileWidth = obj.Width,
                    TileHeight = obj.Height,
                    Image = new TmxImage
                    {
                        Source = !obj.File.ToLower().EndsWith("fire") ?
                                $"../graphics/objects/{obj.File.ToLower()}.0.0.png" :
                                $"../graphics/objects/{obj.File.Replace("fire", "dead").ToLower()}.0.0.png"
                    },
                    FirstGid = 7000 + obj.Id,
                    TileOffset = new TmxTileOffset
                    {
                        X = -obj.CenterX,
                        Y = -obj.CenterY
                    }

                });
            }

            map.Layers.Add(new TmxTileLayer
            {
                Name = "objects",
                Width = (int)toConvert.Data.Width,
                Height = (int)toConvert.Data.Height,
                Visible = true,
                Data = new TmxData
                {
                    Encoding = "csv",
                    Tiles = toConvert.Objects.Select(a =>
                        new TmxDataTile
                        {
                            Gid = a == 0 ? 0 : (uint)a + 7000 - 1,
                        }
                    ).ToList()
                }
            });

            yield return new TmxFile
            {
                Map = map,
                relativeFileExtension = ".tmx",
                relativeFileDirectory = toConvert.relativeFileDirectory,
                relativeFileName = toConvert.relativeFileName
            };
        }
    }
}