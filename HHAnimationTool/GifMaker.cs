using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using AnimatedGif;
namespace HHAnimationTool
{
    internal class GifMaker
    {
        Bitmap[] imgs;
        public List<Bitmap> Gif(string filename, bool double_bg, bool sized24, bool deathOnly, int scaling, bool isHero)
        {


            int scaleWidth;
            int twice = 1;
            if (double_bg)
            {
                scaleWidth = 2;
            }
            else
            {
                scaleWidth = 1;
            }


            //System.Diagnostics.Debug.WriteLine("ok");
            //       Image img=Image.FromFile(filename);

            Bitmap original = new Bitmap(filename);//Format32bppPArgb
            Bitmap clone = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppRgb);
            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(original, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            int cut = 20;
            if (isHero)
            {
                cut = 32;
            }
            int scale = scaling;

            int widthImg = clone.Width / cut;

            if (clone.Height > 24)
            {
                sized24 = false;
            }
            else if (widthImg > 48 && widthImg > clone.Height)
            {
                sized24 = false;
            }
            imgs = new Bitmap[cut];
            Rectangle dstRect;
            Rectangle dstRect2;
            Rectangle dstRect_doublebg;
            Rectangle dstRect_doublebg_2;
            //System.Diagnostics.Debug.WriteLine(clone.Height);
            //System.Diagnostics.Debug.WriteLine(widthImg);
            //System.Diagnostics.Debug.WriteLine((48 - widthImg) * scale);

            if (widthImg > 24)
            {
                twice = 2;
            }
            widthImg /= twice;


            if (sized24)
            {

                dstRect = new Rectangle((24 - widthImg) * scale / 2, (24 - clone.Height) * scale, widthImg * scale, clone.Height * scale);
                dstRect2 = new Rectangle(24 * scale + (24 - widthImg) * scale / 2, (24 - clone.Height) * scale, widthImg * scale, clone.Height * scale);
            }
            else
            {
                dstRect = new Rectangle(0, 0, widthImg * scale, clone.Height * scale);
                dstRect2 = new Rectangle(widthImg * scale, 0, widthImg * scale, clone.Height * scale);
            }

            if (double_bg)
            {
                int shift = widthImg * twice * scale;
                if (sized24)
                {
                    shift = 24 * twice * scale;
                }

                dstRect_doublebg = new Rectangle(dstRect.X + shift, dstRect.Y, dstRect.Width, dstRect.Height);
                dstRect_doublebg_2 = new Rectangle(dstRect2.X + shift, dstRect2.Y, dstRect2.Width, dstRect2.Height);
            }
            else
            {
                dstRect_doublebg = new Rectangle();
                dstRect_doublebg_2 = new Rectangle();


            }
            //System.Diagnostics.Debug.WriteLine(dstRect);
            //System.Diagnostics.Debug.WriteLine(dstRect2);

            Brush brush = new SolidBrush(Color.FromArgb(255, 180, 180, 180));




            for (int i = 0; i < cut; i++)
            {
                if (sized24)
                {
                    imgs[i] = new Bitmap(24 * twice * scale * scaleWidth, 24 * scale, PixelFormat.Format32bppPArgb);
                }
                else
                {
                    imgs[i] = new Bitmap(widthImg * twice * scale * scaleWidth, clone.Height * scale, PixelFormat.Format32bppPArgb);
                }

                //System.Diagnostics.Debug.WriteLine(new Rectangle(i * widthImg, 0, (i + 1) * widthImg, clone.Height));

                using (Graphics graphics = Graphics.FromImage(imgs[i]))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;

                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graphics.SmoothingMode = SmoothingMode.None;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        //graphics.Clear(Color.Gray);

                        //graphics.DrawRectangle(pen, 0, 0, widthImg * scale, clone.Height * scale);

                        graphics.DrawImage(original, dstRect, i * widthImg * twice, 0, widthImg, clone.Height, GraphicsUnit.Pixel, wrapMode);
                        if (twice == 2)
                        {
                            graphics.DrawImage(original, dstRect2, i * widthImg * twice + widthImg, 0, widthImg, clone.Height, GraphicsUnit.Pixel, wrapMode);
                        }


                        if (double_bg)
                        {
                            graphics.CompositingMode = CompositingMode.SourceOver;
                            graphics.FillRectangle(brush, new Rectangle(imgs[i].Width / 2, 0, imgs[i].Width / 2, imgs[i].Height));

                            graphics.DrawImage(original, dstRect_doublebg, i * widthImg * twice, 0, widthImg, clone.Height, GraphicsUnit.Pixel, wrapMode);
                            if (twice == 2)
                            {
                                graphics.DrawImage(original, dstRect_doublebg_2, i * widthImg * twice + widthImg, 0, widthImg, clone.Height, GraphicsUnit.Pixel, wrapMode);
                            }
                        }


                    }

                    //graphics.DrawImage(clone,new Rectangle(0,0,widthImg,clone.Height) ,new Rectangle(i * widthImg, 0, (i + 1) * widthImg, clone.Height),GraphicsUnit.Pixel);

                }
            }
            //imgs[0].Save(filename.Substring(0,filename.Length-4)+" Test.png");

            clone.Dispose();
            original.Dispose();
            List<Bitmap> listToAdd = new List<Bitmap>();

            if (!isHero)
            {



                Bitmap[] idleAnim = new Bitmap[8];
                Bitmap[] walkAnim = new Bitmap[8];
                Bitmap[] attackAnim = new Bitmap[4];
                Bitmap[] hurtAnim = new Bitmap[8];
                Bitmap[] deathAnim = new Bitmap[8];
                for (int i = 0; i < 4; i++)
                {
                    idleAnim[i] = imgs[(i / 2)];
                    walkAnim[i] = imgs[(i / 2) + 4];
                    attackAnim[i] = imgs[i + 8];
                    hurtAnim[i] = imgs[(i / 2) + 12];
                    deathAnim[i] = imgs[(i / 2) + 16];
                }
                for (int i = 4; i < 8; i++)
                {

                    idleAnim[i] = imgs[(i / 2)];
                    walkAnim[i] = imgs[(i / 2) + 4];
                    hurtAnim[i] = imgs[(i / 2) + 12];
                    deathAnim[i] = imgs[(i / 2) + 16];
                }
                List<Bitmap[]> anims = new List<Bitmap[]>
            {
                idleAnim,
                walkAnim,
                attackAnim,
                hurtAnim,
                deathAnim
            };


                int[] animationlist;
                if (deathOnly)
                {
                    animationlist = new int[] { 0, 0, 0,1,1,0,2,0,3, 4 };
                }
                else
                {
                    animationlist = new int[] { 0, 0, 0, 1, 1, 0, 2, 0, 3, 0, 2, 0, 2, 3, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 2, 0, 3, 0, 2, 0, 2, 3, 0, 1, 1, 0, 4 };
                }


                foreach (int idx in animationlist)
                {
                    Bitmap[] a = anims[idx];

                    for (int i = 0; i < a.Length; i++)
                    {
                        listToAdd.Add(a[i]);
                    }
                }
                for (int i = 0; i < 20; i++)
                {
                    listToAdd.Add(deathAnim[deathAnim.Length - 1]);
                }
            }
            else
            {

                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int z = 0; z < 4; z++)
                        {
                            listToAdd.Add(imgs[x * 4 + z]);
                        }
                    }
                }
            }


            return listToAdd;
        }
        public void clear()
        {

            for (int i = 0; i < imgs.Length; i++)
            {
                imgs[i].Dispose();
            }
        }

        //private byte[] GifAnimation = { 33, 255, 11, 78, 69, 84, 83, 67, 65, 80, 69, 50, 46, 48, 3, 1, 0, 0, 0 };
        //private byte[] Delay = { 255, 0 };
        //MemoryStream MS = new MemoryStream();
        //BinaryWriter BW = new BinaryWriter(new FileStream(filenameOut, FileMode.Create));

        //listToAdd[0].Save(MS, ImageFormat.Gif);
        //byte[] B = MS.ToArray();
        //B[10] = (byte)(B[10] & 0X78); //No global color table.
        //BW.Write(B, 0, 13);
        //BW.Write(GifAnimation);
        //WriteGifImg(B, BW);
        //foreach (Bitmap item in listToAdd)
        //{
        //    MS.SetLength(0);
        //    item.Save(MS, ImageFormat.Gif);
        //    B = MS.ToArray();
        //    WriteGifImg(B, BW);
        //}
        //BW.Write(B[B.Length - 1]);
        //BW.Close();
        //MS.Dispose();
        //private void WriteGifImg(byte[] B, BinaryWriter BW)
        //{
        //    B[785] = Delay[0]; //5 secs delay
        //    B[786] = Delay[1];
        //    B[798] = (byte)(B[798] | 0X87);
        //    BW.Write(B, 781, 18);
        //    BW.Write(B, 13, 768);
        //    BW.Write(B, 799, B.Length - 800);
        //}

    }
}
