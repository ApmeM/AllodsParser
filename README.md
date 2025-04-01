B&B box files converter
==========
Console utility to convert original Allods (Rage of mages) resources into unpacked readable formats:

- BMP files unpacked as is with the image inside
- WAV files unpacked as is with the sound inside
- TXT files unpacked as is with the text inside
- REG files unpacked as json text file with the data about images
- 16 files unpacked as image (mostly fonts)
- 16A files unpacked as images
- 256 files unpacked as images
- ALM files converted to [tiled](https://www.mapeditor.org/) .TMX

Usage
==========

1. Download original game iso file (for example from http://allods2.eu)
2. Download maps (for exmple from https://github.com/igroglaz/rom2maps) and put them to maps folder under data folder in allods directory
3. Unpack it (for example to ~/Downloads/Allods/)
4. Clone this repository
5. Change directory to src
6. Run the dotnet command `dotnet run ~/Downloads/Allods/data/ ../Result`

The result:

In ../Result folder new subfolders will be created.
The struture of the folers will be the same as in original folder

Example
=========

![](https://github.com/ApmeM/AllodsParser/raw/main/Example.png)


Credits
==========

- [**UnityAllods**](https://github.com/jewalky/UnityAllods) - Inspiration and the data was taken from there
- [**Rom2Maps**](https://github.com/igroglaz/rom2maps) - Alm game maps examples
