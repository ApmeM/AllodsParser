using SimpleTiled;
using SixLabors.ImageSharp;

namespace AllodsParser
{
    public class AlmToTmxConverter : BaseFileConverter
    {
        public override List<BaseFile> Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<AlmFile>()
                .ToList();

            var newFiles = oldFiles.Select(Convert).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));

            return files;
        }

        private TmxFile Convert(AlmFile toConvert)
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

            // var structuresData = files
            //     .OfType<RegFile>()
            //     .First(a => a.relativeFilePath == "graphics/structures/structures.reg")
            //     .innerData;

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

            return new TmxFile
            {
                Map = map,
                relativeFileExtension = ".tmx",
                relativeFileDirectory = toConvert.relativeFileDirectory,
                relativeFileName = toConvert.relativeFileName
            };
        }
    }
}