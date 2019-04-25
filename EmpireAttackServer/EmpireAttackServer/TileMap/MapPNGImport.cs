using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using EmpireAttackServer.Shared;

namespace EmpireAttackServer.TileMap
{
    public class MapPNGImport : MapBase
    {
        #region Public Constructors

        public MapPNGImport(string path)
        {
            PNGToTileMap(path);
            //DebugMap();
        }

        #endregion Public Constructors

        #region Private Methods

        private void DebugMap()
        {
            if (tileMap == null)
            {
                Console.WriteLine("Error: tileMap is null");
                return;
            }
            for (int i = 0; i < tileMap.Length; i++)
            {
                string print = "";
                for (int j = 0; j < tileMap[0].Length; j++)
                {
                    print += " " + tileMap[i][j].GetShortType();
                }
                Console.WriteLine(print);
            }
        }

        private void PNGToTileMap(string path)
        {
            Bitmap myBitMap = null;
            try
            {
                myBitMap = new Bitmap(path);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Loading Map from PNG failed. Please specify a correct path");
            }

            if (myBitMap == null) { Console.WriteLine("Loading Map from PNG failed. Please specify a correct path"); return; }

            tileMap = new Tile[myBitMap.Width][];
            for (int x = 0; x < myBitMap.Width; x++)
            {
                tileMap[x] = new Tile[myBitMap.Height];
                for (int y = 0; y < myBitMap.Height; y++)
                {
                    // Get the color of a pixel within myBitmap.
                    Color pixelColor = myBitMap.GetPixel(y, x);
                    string pixelColorStringValue =
                        pixelColor.R.ToString("D3") + " " +
                        pixelColor.G.ToString("D3") + " " +
                        pixelColor.B.ToString("D3");
                    switch (pixelColorStringValue)
                    {
                        case "000 000 255":
                            {
                                // blue pixel
                                tileMap[x][y] = new Tile(TileType.Water);
                                break;
                            }
                        case "000 255 000":
                            {
                                // green pixel
                                tileMap[x][y] = new Tile(TileType.Forest);
                                break;
                            }
                        case "255 255 000":
                            {
                                // yellow pixel
                                tileMap[x][y] = new Tile(TileType.Hills);
                                break;
                            }
                        case "255 000 000":
                            {
                                // red pixel
                                tileMap[x][y] = new Tile(TileType.Urban);
                                break;
                            }
                        case "255 255 255":
                            {
                                // white pixel
                                tileMap[x][y] = new Tile(TileType.Normal);
                                break;
                            }
                        case "000 000 000":
                            {
                                // black pixel
                                tileMap[x][y] = new Tile(TileType.Capital);
                                break;
                            }
                    }
                }
            }
        }

        #endregion Private Methods
    }
}