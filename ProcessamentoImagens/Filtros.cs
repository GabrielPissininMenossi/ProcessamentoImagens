using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ProcessamentoImagens
{
    class Filtros
    {
        //variaveis para os pixels vizinhos do shang suen
        private unsafe byte* p1, p2, p3, p4, p5, p6, p7, p8, p9;

        //alguns métodos para pegar os vizinhos de p1
        private unsafe byte* getP2(int x, int y, int width, int height, int pixelSize, int stride, byte* pixelAtual)
        {
            if (x-1 >= 0) //verifica o range da coluna
            {
                //uma posição acima do pixel atual
                byte* p = pixelAtual + ((x - 1) * stride) + (y * pixelSize);

                return p;
            }
            return null;
        } //feito

        private unsafe byte* getP3(int x, int y, int width, int height, int pixelSize, int stride, byte* pixelAtual)
        {
            if (y+1 < width && x-1 >= 0) //verifica o range de coluna e linha
            {
                //pixel acima a direita
                byte* p = pixelAtual + ((x - 1) * stride) + ((y+1) * pixelSize);

                return p;
            }
            return null;
        } //feito
        private unsafe byte* getP4(int x, int y, int width, int height, int pixelSize, int stride, byte* pixelAtual)
        {
            if (y+1 < width)
            {
                //pixel à direita
                byte* p = pixelAtual + (x * stride) + ((y+1) * pixelSize);

                return p;
            }
            return null;
        } //feito
        private unsafe byte* getP5(int x, int y, int width, int height, int pixelSize, int stride, byte* pixelAtual)
        {
            if (y+1 < width && x+1 <= height) //verificar o range de baixo
            {
                //pixel abaixo à direita
                byte* p = pixelAtual + ((x + 1) * stride) + ((y+1) * pixelSize);

                return p;
            }
            return null;
        }
        private unsafe byte* getP6(int x, int y, int width, int height, int pixelSize, int stride, byte* pixelAtual)
        {
            if (x+1 < height) //verificar o range do pixel abaixo
            {
                //pixel abaixo
                byte* p = pixelAtual + ((x + 1) * stride) + (y * pixelSize);

                return p;
            }
            return null;
        }
        private unsafe byte* getP7(int x, int y, int width, int height, int pixelSize, int stride, byte* pixelAtual)
        {
            if (y-1 >= 0 && x+1 < height) //verificar o range do pixel abaixo à esquerda
            {
                //pixel abaixo à esquerda
                byte* p = pixelAtual + ((x + 1) * stride) + ((y-1) * pixelSize);

                return p;
            }
            return null;
        }
        private unsafe byte* getP8(int x, int y, int width, int height, int pixelSize, int stride, byte* pixelAtual)
        {
            if (y-1 >= 0) //verificar o range do pixel ao lado esquerdo
            {
                //pixel à esquerda
                byte* p = pixelAtual + (x * stride) + ((y-1) * pixelSize);

                return p;
            }
            return null;
        }
        private unsafe byte* getP9(int x, int y, int width, int height, int pixelSize, int stride, byte* pixelAtual)
        {
            if (y-1 >= 0 && x-1 >= 0) //verificar o range do pixel acima à esquerda
            {
                //pixel acima à esquerda
                byte* p = pixelAtual + ((x - 1) * stride) + ((y-1) * pixelSize);

                return p;
            }
            return null;
        }

        //sem acesso direto a memoria
        public static void convert_to_gray(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int r, g, b;
            Int32 gs;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //obtendo a cor do pixel
                    Color cor = imageBitmapSrc.GetPixel(x, y);

                    r = cor.R;
                    g = cor.G;
                    b = cor.B;
                    gs = (Int32)(r * 0.2990 + g * 0.5870 + b * 0.1140);

                    //nova cor
                    Color newcolor = Color.FromArgb(gs, gs, gs);

                    imageBitmapDest.SetPixel(x, y, newcolor);
                }
            }
        }

        //sem acesso direito a memoria
        public static void negativo(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int r, g, b;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //obtendo a cor do pixel
                    Color cor = imageBitmapSrc.GetPixel(x, y);

                    r = cor.R;
                    g = cor.G;
                    b = cor.B;

                    //nova cor
                    Color newcolor = Color.FromArgb(255 - r, 255 - g, 255 - b);

                    imageBitmapDest.SetPixel(x, y, newcolor);
                }
            }
        }

        //com acesso direto a memória
        public static void convert_to_grayDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3;
            Int32 gs;

            //lock dados bitmap origem
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //lock dados bitmap destino
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();

                int r, g, b;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        b = *(src++); //está armazenado dessa forma: b g r
                        g = *(src++);
                        r = *(src++);
                        gs = (Int32)(r * 0.2990 + g * 0.5870 + b * 0.1140);
                        *(dst++) = (byte)gs;
                        *(dst++) = (byte)gs;
                        *(dst++) = (byte)gs;
                    }
                    src += padding;
                    dst += padding;
                }
            }
            //unlock imagem origem
            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            //unlock imagem destino
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }

        //com acesso direito a memoria
        public static void negativoDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3;

            //lock dados bitmap origem 
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //lock dados bitmap destino
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            unsafe
            {
                byte* src1 = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();

                int r, g, b;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        b = *(src1++); //está armazenado dessa forma: b g r 
                        g = *(src1++);
                        r = *(src1++);

                        *(dst++) = (byte)(255 - b);
                        *(dst++) = (byte)(255 - g);
                        *(dst++) = (byte)(255 - r);
                    }
                    src1 += padding;
                    dst += padding;
                }
            }
            //unlock imagem origem 
            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            //unlock imagem destino
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }

        public static void esqueletizarDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3;

            //travar as imagens de origem e destino
            //lock dados bitmap origem 
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //lock dados bitmap destino
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            //começar o algoritmo zhang suen com DMA (Direct Memory Access)
            unsafe
            {
                //colocar os ponteiros nas primeiras posições das imagens de origem e destino
                byte* origem = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* destino = (byte*)bitmapDataDst.Scan0.ToPointer();

                //rgb para armazenar os valores de cada pixel (apenas 1 byte)
                int r, g, b;

                //agora irei percorrer todos os pixels com 2 for's

            }

            //destravar as imagens de origem e destino
            //unlock imagem origem 
            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            //unlock imagem destino
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }
    }
}
