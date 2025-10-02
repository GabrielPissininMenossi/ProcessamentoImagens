using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows.Forms;

namespace ProcessamentoImagens
{
    class Filtros
    {
        //variaveis para os pixels vizinhos do shang suen
        private unsafe byte* p1, p2, p3, p4, p5, p6, p7, p8, p9;

        //nos meus métodos de GetPn, x é a linha e y é a coluna
        //alguns métodos para pegar os vizinhos de p1
        private static unsafe byte* GetP2(int x, int y, int width, int height, int pixelSize, int stride, byte* basePixel)
        {
            if (x - 1 >= 0) //verifica o range da coluna
            {
                //uma posição acima do pixel atual
                byte* p = basePixel + ((x - 1) * stride) + (y * pixelSize);

                return p;
            }
            return null;
        }
        private static unsafe byte* GetP3(int x, int y, int width, int height, int pixelSize, int stride, byte* basePixel)
        {
            if (y + 1 < width && x - 1 >= 0) //verifica o range de coluna e linha
            {
                //pixel acima a direita
                byte* p = basePixel + ((x - 1) * stride) + ((y + 1) * pixelSize);

                return p;
            }
            return null;
        }
        private static unsafe byte* GetP4(int x, int y, int width, int height, int pixelSize, int stride, byte* basePixel)
        {
            if (y + 1 < width)
            {
                //pixel à direita
                byte* p = basePixel + (x * stride) + ((y + 1) * pixelSize);

                return p;
            }
            return null;
        }
        private static unsafe byte* GetP5(int x, int y, int width, int height, int pixelSize, int stride, byte* basePixel)
        {
            if (y + 1 < width && x + 1 < height) //verificar o range de baixo
            {
                //pixel abaixo à direita
                byte* p = basePixel + ((x + 1) * stride) + ((y + 1) * pixelSize);

                return p;
            }
            return null;
        }
        private static unsafe byte* GetP6(int x, int y, int width, int height, int pixelSize, int stride, byte* basePixel)
        {
            if (x + 1 < height) //verificar o range do pixel abaixo
            {
                //pixel abaixo
                byte* p = basePixel + ((x + 1) * stride) + (y * pixelSize);

                return p;
            }
            return null;
        }
        private static unsafe byte* GetP7(int x, int y, int width, int height, int pixelSize, int stride, byte* basePixel)
        {
            if (y - 1 >= 0 && x + 1 < height) //verificar o range do pixel abaixo à esquerda
            {
                //pixel abaixo à esquerda
                byte* p = basePixel + ((x + 1) * stride) + ((y - 1) * pixelSize);

                return p;
            }
            return null;
        }
        private static unsafe byte* GetP8(int x, int y, int width, int height, int pixelSize, int stride, byte* basePixel)
        {
            if (y - 1 >= 0) //verificar o range do pixel ao lado esquerdo
            {
                //pixel à esquerda
                byte* p = basePixel + (x * stride) + ((y - 1) * pixelSize);

                return p;
            }
            return null;
        }
        private static unsafe byte* GetP9(int x, int y, int width, int height, int pixelSize, int stride, byte* basePixel)
        {
            if (y - 1 >= 0 && x - 1 >= 0) //verificar o range do pixel acima à esquerda
            {
                //pixel acima à esquerda
                byte* p = basePixel + ((x - 1) * stride) + ((y - 1) * pixelSize);

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
            int width = imageBitmapSrc.Width;//quant. pixels
            int height = imageBitmapSrc.Height;//quant. pixels
            int pixelSize = 3; //número de bytes por pixel

            //lock dados bitmap origem 
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //lock dados bitmap destino
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //largura da linha em bytes - (largura em pixels * 3)
            int padding = bitmapDataSrc.Stride - (width * pixelSize);
            //stride - largura de uma linha em bytes 

            unsafe //ignora os recursos padrão de segurança de memória gerenciada
            {
                byte* src1 = (byte*)bitmapDataSrc.Scan0.ToPointer();//endereço inicial dos dados do pixel
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
                    //garante que o loop não tente processar os bytes de preenchimento como se fossem dados de pixel
                    src1 += padding;
                    dst += padding;
                }
            }
            //unlock imagem origem 
            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            //unlock imagem destino
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }





        //exercícios para estudo para a prova -> Sem DMA
        public static void espelho_vertical(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //pego o pixel da imagem de origem
                    Color cor = imageSrc.GetPixel(x, y);

                    //seto o pixel atual de origem na minha imagem destino
                    //faço a conta simples para o espelho e tiro 1 para não acessar posição fora do range
                    imageDst.SetPixel(width-x-1, y, cor);
                }
            }
        } 

        public static void espelho_horizontal(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            Color cor;
            for(int y=0; y<height; y++)
            {
                for(int x=0; x<width; x++)
                {
                    cor = imageSrc.GetPixel(x, y); //pego o origem
                    imageDst.SetPixel(x, height-y-1, cor); //seto no destino realizando o cálculo
                }
            }
        }

        public static void canal_azul(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            int r, g, b;
            Color cor;

            for(int y=0; y<height; y++)
            {
                for(int x=0; x<width; x++)
                {
                    cor = imageSrc.GetPixel(x, y);
                    b = cor.B;
                    g = cor.G;
                    r = cor.R;
                    imageDst.SetPixel(x, y, Color.FromArgb(0, 0, b) );
                }
            }
        }

        public static void canal_verde(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            int r, g, b;
            Color cor;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    cor = imageSrc.GetPixel(x, y);
                    b = cor.B;
                    g = cor.G;
                    r = cor.R;
                    imageDst.SetPixel(x, y, Color.FromArgb(0, g, 0));
                }
            }
        }

        public static void canal_vermelho(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            int r, g, b;
            Color cor;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    cor = imageSrc.GetPixel(x, y);
                    b = cor.B;
                    g = cor.G;
                    r = cor.R;
                    imageDst.SetPixel(x, y, Color.FromArgb(r, 0, 0));
                }
            }
        }

        public static void preto_branco(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            int r, g, b;
            Color cor;

            for(int y=0; y<height; y++)
            {
                for(int x=0; x<width; x++)
                {
                    cor = imageSrc.GetPixel(x, y);
                    b = cor.B;
                    g = cor.G;
                    r = cor.R;
                    if((b+g+r)/3 < 128) //então ele será preto
                    {
                        b = 0;
                        g = 0;
                        r = 0;
                    }
                    else
                    {
                        b = 255;
                        g = 255;
                        r = 255;
                    }
                    imageDst.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
        }

        public static void noventa(Bitmap imageSrc, Bitmap imageDst)
        {
            int widthSrc = imageSrc.Width;
            int heightSrc = imageSrc.Height;
            int widthDst = imageDst.Width;
            int heightDst = imageDst.Height;
            Color cor;

            for(int y=0; y<heightSrc; y++)
            {
                for(int x=0; x<widthSrc; x++)
                {
                    cor = imageSrc.GetPixel(x, y);

                    imageDst.SetPixel(widthDst-y-1, x, cor);
                }
            }
        }

        public static void inverte_azul_vermelho(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            int r, g, b;
            Color cor;
            
            for(int y=0; y<height; y++)
            {
                for(int x=0; x<width; x++)
                {
                    cor = imageSrc.GetPixel(x, y);
                    r = cor.R;
                    g = cor.G;
                    b = cor.B;

                    //fromArgb é na ordem r g b -> mas eu faço b g r para inverter os canais
                    imageDst.SetPixel(x, y, Color.FromArgb(b,g,r));
                }
            }
        }

        public static void inverte_diagonal(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            Color cor;

            for(int y=0; y<height; y++)
            {
                for(int x=0; x<width; x++)
                {
                    cor = imageSrc.GetPixel(x, y);

                    //fazer as contas e mandar para o set pixel
                    imageDst.SetPixel(width-1-x, height-1-y, cor);
                }
            }
        }

        public static void racha_quatro(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            Color cor;

            //primeiro quadrate da origem
            //for da metade de cima esquerda
            for(int y=0; y<height/2; y++)
            {
                for(int x=0; x<width/2; x++)
                {
                    //vai ficar no quarto quadrante do meu destino
                    cor = imageSrc.GetPixel(x, y);
                    imageDst.SetPixel(width/2+x, height/2+y, cor);
                }
            } //feito

            //segundo quadrante da origem
            //for da metade de cima direita
            for (int y = 0; y < height/2; y++)
            {
                for (int x = width/2; x < width; x++)
                {
                    //vai ficar no terceiro quadrante do meu destino
                    cor = imageSrc.GetPixel(x, y);
                    imageDst.SetPixel(x-(width/2), height / 2 + y, cor);
                }
            } //feito

            //terceiro quadrante da origem
            //for da metade de baixo esquerda
            for (int y = height/2; y < height; y++)
            {
                for (int x = 0; x < width/2; x++)
                {
                    //vai ficar no segundo quadrante do meu destino
                    cor = imageSrc.GetPixel(x, y);
                    imageDst.SetPixel(width / 2 + x, y-(height/2), cor);
                }
            } //feito

            //quarto quadrante da origem
            //for da metade de baixo direita
            for (int y = height/2; y < height; y++)
            {
                for (int x = width/2; x < width; x++)
                {
                    //vai ficar no primeiro quadrante do meu destino
                    cor = imageSrc.GetPixel(x, y);
                    imageDst.SetPixel(x-(width / 2), y-(height/2), cor);
                }
            } //feito
        }


        //com DMA

        public static void espelho_verticalDMA(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            int pixelSize = 3;

            BitmapData bitmapDataSrc = imageSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            BitmapData bitmapDataDst = imageDst.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            //stride = linha em bytes da img.
            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();
                byte* dstAux;

                int stride = bitmapDataDst.Stride;

                int r, g, b;
                for(int y = 0; y < height; y++)
                {
                    for(int x = 0; x < width; x++)
                    {
                        b = *(src++);
                        g = *(src++);
                        r = *(src++);


                        //acessar linha
                        dstAux = dst + (y * stride);

                        //acessar coluna
                        dstAux += (width - 1 - x) * pixelSize;

                        *(dstAux++) = (byte)b;
                        *(dstAux++) = (byte)g;
                        *(dstAux++) = (byte)r;

                        //byte* p = basePixel + ((x - 1) * stride) + (y * pixelSize);
                    }
                    src += padding;
                }
            }
            imageSrc.UnlockBits(bitmapDataSrc);
            imageDst.UnlockBits(bitmapDataDst);
        }

        public static void espelho_horizontalDMA(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            int pixelSize = 3;

            BitmapData bitmapDataSrc = imageSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            BitmapData bitmapDataDst = imageDst.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();
                byte* auxDst;

                int r, g, b;

                int stride = bitmapDataDst.Stride;


                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        b = *(src++);
                        g = *(src++);
                        r = *(src++);

                        //coluna 
                        auxDst = dst + (x * pixelSize);

                        //linha
                        auxDst += (height - 1 - y) * stride;


                        *(auxDst++) = (byte)b;
                        *(auxDst++) = (byte)g;
                        *(auxDst++) = (byte)r;


                    }
                    src += padding;
                }
            }
            imageSrc.UnlockBits(bitmapDataSrc);
            imageDst.UnlockBits(bitmapDataDst);
        }

        public static void canal_corDMA(Bitmap imageSrc, Bitmap imageDst, int azul, int vermelho, int verde)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            int pixelSize = 3;
            int r, g, b;

            BitmapData bitmapDataSrc = imageSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            BitmapData bitmapDataDst = imageDst.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();


                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        b = *(src++);
                        g = *(src++);
                        r = *(src++);

                        if(azul == 1)
                            g = r = 0;
                        else if (vermelho == 1)
                                g = b = 0;
                        else if(verde == 1)
                                b = r = 0;
                        
                            *(dst++) = (byte)b;
                        *(dst++) = (byte)g;
                        *(dst++) = (byte)r;
                    }
                    src += padding;
                    dst += padding;
                }
            }
            imageSrc.UnlockBits(bitmapDataSrc);
            imageDst.UnlockBits(bitmapDataDst);
        }

        public static void preto_brancoDMA(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            int pixelSize = 3;
            int r, g, b;


            BitmapData bitmapDataSrc = imageSrc.LockBits(new Rectangle(0, 0, width, height),
               ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            BitmapData bitmapDataDst = imageDst.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {

                        b = *(src++);
                        g = *(src++);
                        r = *(src++);

                        if ((b + g + r) / 3 < 128) //então ele será preto
                        {
                            b = g = r = 0;
                        }
                        else
                        {
                            b = g = r = 255;
                        }
                        *(dst++) = (byte)b;
                        *(dst++) = (byte)g;
                        *(dst++) = (byte)r;
                    }
                    dst += padding;
                    src += padding;
                }
            }
            imageSrc.UnlockBits(bitmapDataSrc);
            imageDst.UnlockBits(bitmapDataDst);
        }

        public static void noventaDMA(Bitmap imageSrc, Bitmap imageDst)
        {
            int widthSrc = imageSrc.Width;
            int heightSrc = imageSrc.Height;
            int widthDst = imageDst.Width;
            int heightDst = imageDst.Height;
            int pixelSize = 3;

            BitmapData bitmapDataSrc = imageSrc.LockBits(new Rectangle(0, 0, widthSrc, heightSrc),
               ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            BitmapData bitmapDataDst = imageDst.LockBits(new Rectangle(0, 0, widthDst, heightDst),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int paddingSrc = bitmapDataSrc.Stride - (widthSrc * pixelSize);
            int paddingDst = bitmapDataDst.Stride - (widthDst * pixelSize);

            int strideSrc = bitmapDataSrc.Stride;
            int strideDst = bitmapDataDst.Stride;

            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();
                byte* AuxDst;

                int r, g, b;
                for (int y = 0; y < heightSrc; y++)
                {
                    for (int x = 0; x < widthSrc; x++)
                    {
                        b = *(src++);
                        g = *(src++);
                        r = *(src++);


                        //linha destino
                        AuxDst = dst + (x * strideDst);

                        //coluna destino
                        AuxDst += (widthDst - y) * pixelSize;


                        *(AuxDst++) = (byte)b;
                        *(AuxDst++) = (byte)g;
                        *(AuxDst++) = (byte)r;
                    }
                    src += paddingSrc;
                }
            }
            imageSrc.UnlockBits(bitmapDataSrc);
            imageDst.UnlockBits(bitmapDataDst);
        }

        public static void inverte_azul_vermelhoDMA(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            int pixelSize = 3;
            int r, g, b;

            BitmapData bitmapDataSrc = imageSrc.LockBits(new Rectangle(0, 0, width, height),
               ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            BitmapData bitmapDataDst = imageDst.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);
            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        b = *(src++);
                        g = *(src++);
                        r = *(src++);

                        *(dst++) = (byte)r;//vermelho no azul
                        *(dst++) = (byte)g;
                        *(dst++) = (byte)b;//azul no vermelho

                    }
                    src += padding;
                    dst += padding;
                }

            }
            imageSrc.UnlockBits(bitmapDataSrc);
            imageDst.UnlockBits(bitmapDataDst);
        }


        public static void inverte_diagonalDMA(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            int pixelSize = 3;

            BitmapData bitmapDataSrc = imageSrc.LockBits(new Rectangle(0, 0, width, height),
               ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            BitmapData bitmapDataDst = imageDst.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            int stride = bitmapDataSrc.Stride;

            unsafe
            {
                int b, g, r;

                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();
                byte* Auxdst;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        b = (*src++);
                        g = (*src++);
                        r = (*src++);

                        //calcula linha
                        Auxdst = dst + ((height - y) * stride);

                        //calcula coluna
                        Auxdst += ((width - x) * pixelSize);



                        *(Auxdst++) = (byte)b;
                        *(Auxdst++) = (byte)g;
                        *(Auxdst++) = (byte)r;
                    }
                    src += padding;
                }
            }
            imageSrc.UnlockBits(bitmapDataSrc);
            imageDst.UnlockBits(bitmapDataDst);
        }

        public static void racha_quatroDMA(Bitmap imageSrc, Bitmap imageDst)
        {
            int width = imageSrc.Width;
            int height = imageSrc.Height;
            int pixelSize = 3;

            BitmapData bitmapDataSrc = imageSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            BitmapData bitmapDataDst = imageDst.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            int stride = bitmapDataDst.Stride;

            unsafe
            {
                byte *src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte *dst = (byte*)bitmapDataDst.Scan0.ToPointer();
                byte *srcAux, dstAux;

                int b, g, r;
                //primeiro quadrate da origem
                //for da metade de cima esquerda
                for (int y = 0; y < height / 2; y++)
                {
                    for (int x = 0; x < width / 2; x++)
                    {
                        //coluna
                        srcAux = src + (x * pixelSize);
                        //linha
                        srcAux += stride * y;

                        b = *(srcAux++);
                        g = *(srcAux++);
                        r = *(srcAux++);

                        //metade da linha
                        dstAux = dst + ((height / 2) + y) * stride;

                        dstAux += ((width/2)+x) *pixelSize;

                        *(dstAux++) = (byte)b;
                        *(dstAux++) = (byte)g;
                        *(dstAux++) = (byte)r;
                    }
                } //FEITO

                //segundo quadrante da origem
                //for da metade de cima direita
                for (int y = 0; y < height / 2; y++)
                {
                    for (int x = width / 2; x < width; x++)
                    {
                        //coluna
                        srcAux = src + (x * pixelSize);
                        //linha
                        srcAux += stride * y;

                        b = *(srcAux++);
                        g = *(srcAux++);
                        r = *(srcAux++);

                        //metade da linha
                        dstAux = dst + ((height / 2) + y) * stride;

                        //metade da coluna
                        dstAux += (x-(width/2)) * pixelSize;

                        *(dstAux++) = (byte)b;
                        *(dstAux++) = (byte)g;
                        *(dstAux++) = (byte)r;
                    }
                } //FEITO

                //terceiro quadrante da origem
                //for da metade de baixo esquerda
                for (int y = height / 2; y < height; y++)
                {
                    for (int x = 0; x < width / 2; x++)
                    {
                        //coluna
                        srcAux = src + (x * pixelSize);
                        //linha
                        srcAux += stride * y;

                        b = *(srcAux++);
                        g = *(srcAux++);
                        r = *(srcAux++);

                        //metade da linha
                        dstAux = dst + (y-(height/2)) * stride;

                        //metade coluna
                        dstAux += ((width/2)+ x)* pixelSize;

                        *(dstAux++) = (byte)b;
                        *(dstAux++) = (byte)g;
                        *(dstAux++) = (byte)r;
                    }
                } //FEITO

                //quarto quadrante da origem
                //for da metade de baixo direita
                for (int y = height / 2; y < height; y++)
                {
                    for (int x = width / 2; x < width; x++)
                    {
                        //coluna
                        srcAux = src + (x * pixelSize);
                        //linha
                        srcAux += stride * y;

                        b = *(srcAux++);
                        g = *(srcAux++);
                        r = *(srcAux++);

                        //metade da linha
                        dstAux = dst + (y - (height / 2)) * stride;

                        dstAux += (x - (width / 2)) * pixelSize;

                        *(dstAux++) = (byte)b;
                        *(dstAux++) = (byte)g;
                        *(dstAux++) = (byte)r;
                    }
                } //FEITO
            }
            imageSrc.UnlockBits(bitmapDataSrc);
            imageDst.UnlockBits(bitmapDataDst);
        }






        //metodo para auxiliar no algoritmo de zhang suen
        private static unsafe int Conectividade(byte* p2, byte* p3, byte* p4, byte* p5, byte* p6, byte* p7, byte* p8, byte* p9)
        {
            int v2 = (p2 != null && p2[0] * p2[1] * p2[2] / 3 < 128) ? 1 : 0; //fazer essa média para todos os pixels restantes
            int v3 = (p3 != null && p3[0] * p3[1] * p3[2] / 3 < 128) ? 1 : 0;
            int v4 = (p4 != null && p4[0] * p4[1] * p4[2] / 3 < 128) ? 1 : 0;
            int v5 = (p5 != null && p5[0] * p5[1] * p5[2] / 3 < 128) ? 1 : 0;
            int v6 = (p6 != null && p6[0] * p6[1] * p6[2] / 3 < 128) ? 1 : 0;
            int v7 = (p7 != null && p7[0] * p7[1] * p7[2] / 3 < 128) ? 1 : 0;
            int v8 = (p8 != null && p8[0] * p8[1] * p8[2] / 3 < 128) ? 1 : 0;
            int v9 = (p9 != null && p9[0] * p9[1] * p9[2] / 3 < 128) ? 1 : 0;

            //realizar a soma das transições de 0(branco) para 1(preto)
            int conectividade = 0;
            if (v2 == 0 && v3 == 1) conectividade++;
            if (v3 == 0 && v4 == 1) conectividade++;
            if (v4 == 0 && v5 == 1) conectividade++;
            if (v5 == 0 && v6 == 1) conectividade++;
            if (v6 == 0 && v7 == 1) conectividade++;
            if (v7 == 0 && v8 == 1) conectividade++;
            if (v8 == 0 && v9 == 1) conectividade++;
            if (v9 == 0 && v2 == 1) conectividade++;

            return conectividade;
        }

        private static unsafe int ContaPretos(byte* p2, byte* p3, byte* p4, byte* p5, byte* p6, byte* p7, byte* p8, byte* p9)
        {
            int pretos = 0;

            //tranformar em preto ou branco
            int v2 = (p2[0] * p2[1] * p2[2] / 3 < 128) ? 1 : 0;
            int v3 = (p3[0] * p3[1] * p3[2] / 3 < 128) ? 1 : 0;
            int v5 = (p5[0] * p5[1] * p5[2] / 3 < 128) ? 1 : 0;
            int v4 = (p4[0] * p4[1] * p4[2] / 3 < 128) ? 1 : 0;
            int v6 = (p6[0] * p6[1] * p6[2] / 3 < 128) ? 1 : 0;
            int v7 = (p7[0] * p7[1] * p7[2] / 3 < 128) ? 1 : 0;
            int v8 = (p8[0] * p8[1] * p8[2] / 3 < 128) ? 1 : 0;
            int v9 = (p9[0] * p9[1] * p9[2] / 3 < 128) ? 1 : 0;
            pretos = v2 + v3 + v4 + v5 + v6 + v7 + v8 + v9;
            return pretos;
        }

        private static unsafe int ContaFundo(byte* p1, byte* p2, byte* p3)
        {
            int fundo = 0;
            int v2 = (p1[0] * p1[1] * p1[2] / 3 < 128) ? 1 : 0;
            int v4 = (p2[0] * p2[1] * p2[2] / 3 < 128) ? 1 : 0;
            int v8 = (p3[0] * p3[1] * p3[2] / 3 < 128) ? 1 : 0;
            fundo = v2 * v4 * v8; //com a multiplicação, se algum for branco então retorna 0
            return fundo;
        }

        //algoritmo do zhang suen com DMA
        public static void EsqueletizarDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3, numeroConexao, pretos, qtdRem, r, g, b;
            int[] removerX = new int[width * height], removerY = new int[width * height];

            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);


            //tranformar para preto e branco primeiro
            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();
                byte* aux1, aux2;
                for (int x = 0; x < imageBitmapSrc.Height; x++)
                {
                    for (int y = 0; y < imageBitmapSrc.Width; y++)
                    {
                        aux1 = src + x * bitmapDataSrc.Stride + y * pixelSize;
                        aux2 = dst + x * bitmapDataDst.Stride + y * pixelSize;

                        int media = (aux1[0] + aux1[1] + aux1[2]) / 3;
                        if (media < 128)
                        {
                            aux1[0] = 0;
                            aux1[1] = 0;
                            aux1[2] = 0;
                            aux2[0] = 0;
                            aux2[1] = 0;
                            aux2[2] = 0;
                        }
                        else
                        {
                            aux1[0] = 255;
                            aux1[1] = 255;
                            aux1[2] = 255;
                            aux2[0] = 255;
                            aux2[1] = 255;
                            aux2[2] = 255;
                        }
                    }
                }
            }

            //começar o algoritmo zhang suen com DMA (Direct Memory Access)
            unsafe
            {
                //colocar os ponteiros nas primeiras posições das imagens de origem e destino
                byte* origem = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* destino = (byte*)bitmapDataDst.Scan0.ToPointer();
                byte* pixelAtual, oriAux, destAux;
                byte* p2, p3, p4, p5, p6, p7, p8, p9;

                //agora irei percorrer todos os pixels com 2 for's
                Boolean flag = true;
                while (flag)
                {
                    qtdRem = 0;
                    for (int x = 0; x < height; x++)
                    {
                        for (int y = 0; y < width; y++)
                        {
                            pixelAtual = origem + x * bitmapDataSrc.Stride + y * pixelSize;
                            //b = pixelAtual[0];
                            b = *(pixelAtual++);
                            //g = pixelAtual[1];
                            g = *(pixelAtual++);
                            //r = pixelAtual[2];
                            r = *(pixelAtual++);
                            if (r < 127 && g < 127 && b < 127)
                            {
                                p2 = GetP2(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p3 = GetP3(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p4 = GetP4(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p5 = GetP5(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p6 = GetP6(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p7 = GetP7(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p8 = GetP8(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p9 = GetP9(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);

                                numeroConexao = Conectividade(p2, p3, p4, p5, p6, p7, p8, p9);

                                if (numeroConexao == 1) //verificação do primeiro passo
                                {
                                    pretos = ContaPretos(p2, p3, p4, p5, p6, p7, p8, p9);
                                    if (pretos > 1 && pretos < 7) //verificação do segundo passo (vizinhos pretos)
                                    {
                                        if (ContaFundo(p2, p4, p8) == 0) //verificação do terceiro passo, se verdade, ao menos algum é branco
                                        {
                                            if (ContaFundo(p2, p6, p8) == 0) //verificação do quarto passo, se verdade, ao menos algum é branco
                                            {
                                                removerX[qtdRem] = x;
                                                removerY[qtdRem] = y;
                                                qtdRem++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //agora vou deletar os pixels marcados após essa primeira iteração
                    for (int i = 0; i < qtdRem; i++)
                    {
                        int x = removerX[i];
                        int y = removerY[i];
                        oriAux = origem + x * bitmapDataSrc.Stride + y * pixelSize;
                        destAux = destino + x * bitmapDataDst.Stride + y * pixelSize;
                        *(destAux++) = 255;
                        *(destAux++) = 255;
                        *(destAux++) = 255;
                        *(oriAux++) = 255;
                        *(oriAux++) = 255;
                        *(oriAux++) = 255;
                    }

                    //agora vou para a SEGUNDA ITERAÇÃO
                    qtdRem = 0;
                    for (int x = 0; x < height; x++)
                    {
                        for (int y = 0; y < width; y++)
                        {
                            //pego o pixel atual
                            pixelAtual = origem + x * bitmapDataSrc.Stride + y * pixelSize;
                            //b = pixelAtual[0];
                            b = *(pixelAtual++);
                            //g = pixelAtual[1];
                            g = *(pixelAtual++);
                            //r = pixelAtual[2];
                            r = *(pixelAtual++);

                            if (b < 127 && g < 127 && r < 127) //se achar um pixel preto
                            {
                                p2 = GetP2(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p3 = GetP3(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p4 = GetP4(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p5 = GetP5(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p6 = GetP6(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p7 = GetP7(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p8 = GetP8(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);
                                p9 = GetP9(x, y, width, height, pixelSize, bitmapDataSrc.Stride, origem);

                                numeroConexao = Conectividade(p2, p3, p4, p5, p6, p7, p8, p9);

                                if (numeroConexao == 1) //descobri o número de conexões
                                {
                                    pretos = ContaPretos(p2, p3, p4, p5, p6, p7, p8, p9);
                                    if (pretos > 1 && pretos < 7) //contei os vizinhos pretos
                                    {
                                        if (ContaFundo(p2, p4, p6) == 0) //se verdade, pelo menos algum é branco
                                        {
                                            if (ContaFundo(p4, p6, p8) == 0) //se verdade, pelo menos algum é branco
                                            {
                                                removerX[qtdRem] = x;
                                                removerY[qtdRem] = y;
                                                qtdRem++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (qtdRem == 0) //não tenho mais ninguém para remover e o algoritmo deve terminar
                    {
                        flag = false;
                    }
                    else //removo os pixels marcados e depois retorno para o meu while
                    {
                        for (int i = 0; i < qtdRem; i++)
                        {
                            int x = removerX[i];
                            int y = removerY[i];
                            oriAux = origem + x * bitmapDataSrc.Stride + y * pixelSize;
                            destAux = destino + x * bitmapDataDst.Stride + y * pixelSize;
                            *(destAux++) = 255;
                            *(destAux++) = 255;
                            *(destAux++) = 255;
                            *(oriAux++) = 255;
                            *(oriAux++) = 255;
                            *(oriAux++) = 255;
                        }
                    }
                }
            }

            //destravar as imagens de origem e destino
            //unlock imagem origem
            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            //unlock imagem destino
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }

        //BORDA COUNTING FOLLOWING ----------------------------------------------------------------------------------------
        private static unsafe Boolean DireitaIsPreto(byte* pixelAtual)
        {
            return IsPreto(pixelAtual+3);
        }

        private static unsafe void MarcarPixel(byte* pixel)
        {
            pixel[0] = 0; //B
            pixel[1] = 0; //G
            pixel[2] = 255; //R -> deixo como inteiramente vermelho
        }

        private static unsafe bool Marcado(byte* pixel)
        {
            return pixel[0] == 0 && pixel[1] == 0 && pixel[2] == 255;
        }

        private static unsafe bool IsPreto(byte* pixel)
        {
            return pixel[0] == 0 && pixel[1] == 0 && pixel[2] == 0;
        }

        private static unsafe bool IsBranco(byte* pixel)
        {
            return pixel[0] == 255 && pixel[1] == 255 && pixel[2] == 255;
        }

        private static unsafe byte* GetP(int linha, int coluna, int width, int height, int stride, int pixelSize, byte* src)
        {
            //retorna o pixel na posicão (linha, coluna)
            if (linha>=0 && linha<height && coluna>=0 && coluna<width)
            {
                //o src deve apontar para o pixel (0,0) da imagem com a qual eu quero trabalhar
                return src + linha*stride + coluna*pixelSize;
            }
            return null;
        }

        private static unsafe void AcharPosicao(int[] proximos, int xAtual, int yAtual, int width, int height, int stride, int pixelSize, byte* src, int* direcao)
        {
            //conectividade 8, pegar todos eles
            byte* p2 = GetP(yAtual-1, xAtual, width, height, stride, pixelSize, src); //acima
            byte* p1 = GetP(yAtual-1, xAtual+1, width, height, stride, pixelSize, src); //acima direita
            byte* p0 = GetP(yAtual, xAtual+1, width, height, stride, pixelSize, src); //direita
            byte* p7 = GetP(yAtual+1, xAtual+1, width, height, stride, pixelSize, src); //abaixo direita
            byte* p6 = GetP(yAtual+1, xAtual, width, height, stride, pixelSize, src); //abaixo
            byte* p5 = GetP(yAtual+1, xAtual-1, width, height, stride, pixelSize, src); //abaixo esquerda
            byte* p4 = GetP(yAtual, xAtual-1, width, height, stride, pixelSize, src); //esquerda
            byte* p3 = GetP(yAtual-1, xAtual-1, width, height, stride, pixelSize, src); //acima esquerda
            proximos[0] = -1; proximos[1] = -1; //deixar como inválido inicialmente
            byte*[] pixels = new byte*[8] { p0, p1, p2, p3, p4, p5, p6, p7 };
            bool achou = false;

            for (int i=0; i<8 && !achou; i++)
            {
                int indice = (*direcao + i + 1) % 8;

                if (pixels[indice]!=null && IsPreto(pixels[indice]))
                {
                    int indiceAnterior = (indice + 7) % 8;
                    if(pixels[indiceAnterior]!=null)
                    {
                        switch (indiceAnterior)
                        {
                            case 0: //direita
                                proximos[0] = xAtual + 1; //coluna
                                proximos[1] = yAtual; //linha
                                break;
                            case 1: //acima direita
                                proximos[0] = xAtual + 1; //coluna
                                proximos[1] = yAtual - 1; //linha
                                break;
                            case 2: //acima
                                proximos[0] = xAtual; //coluna
                                proximos[1] = yAtual - 1; //linha
                                break;
                            case 3: //acima esquerda
                                proximos[0] = xAtual - 1; //coluna
                                proximos[1] = yAtual - 1; //linha
                                break;
                            case 4: //esquerda
                                proximos[0] = xAtual - 1; //coluna
                                proximos[1] = yAtual; //linha
                                break;
                            case 5: //abaixo esquerda
                                proximos[0] = xAtual - 1; //coluna
                                proximos[1] = yAtual + 1; //linha
                                break;
                            case 6: //abaixo
                                proximos[0] = xAtual; //coluna
                                proximos[1] = yAtual + 1; //linha
                                break;
                            case 7: //abaixo direita
                                proximos[0] = xAtual + 1; //coluna
                                proximos[1] = yAtual + 1; //linha
                                break;
                        }
                        *direcao = (indiceAnterior + 4) % 8;
                        achou = true;
                    }
                }
            }
        }

        public static unsafe void ContornoDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3;

            //lock bits
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bitmapDataSrc.Stride;
            int padding = stride - (width * pixelSize);

            //fazer aqui os dois fors com o while do contorno
            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer(); //ponteiro para o (0,0) de fonte
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer(); //ponteiro para o (0,0) de destino
                
                //binarizar a entrada -> preto e branco
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte* srcBi = src + y*stride + x*pixelSize;
                        if ((srcBi[0]+srcBi[1]+srcBi[2])/3 < 128) //preto
                        {
                            *(srcBi++) = 0;
                            *(srcBi++) = 0;
                            *(srcBi++) = 0;
                        }
                        else //branco
                        {
                            *(srcBi++) = 255;
                            *(srcBi++) = 255;
                            *(srcBi++) = 255;
                        }
                    }
                }

                //inicializar o destino -> deixar tudo branco
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte* dstInit = dst + y*stride + x*pixelSize;
                        //branco
                        *(dstInit++) = 255;
                        *(dstInit++) = 255;
                        *(dstInit++) = 255;
                    }
                }

                //contorno
                for (int y=0; y<height; y++)
                {
                    for(int x=0; x<width; x++)
                    {
                        byte* pixelAtual = src + y*stride + x*pixelSize;
                        byte* direita = GetP(y, x+1, width, height, stride, pixelSize, src); //pega o pixel da direita
                        if (IsBranco(pixelAtual) && direita!=null && IsPreto(direita) && !Marcado(pixelAtual))
                        {
                            byte* inicial = pixelAtual; //define o primeiro pixel do contorno
                            byte* atualContorno = inicial; //define o pixel atual do contorno
                            int xAtual = x, yAtual = y;
                            int[] proximos = new int[2];
                            int direcao = 4;
                            do
                            {
                                MarcarPixel(atualContorno); //marcar o pixel atual como parte do contorno
                                AcharPosicao(proximos, xAtual, yAtual, width, height, stride, pixelSize, src, &direcao); //achar o próximo pixel do contorno
                                if (proximos[0]!=-1)
                                {
                                    xAtual = proximos[0]; //coluna
                                    yAtual = proximos[1]; //linha
                                    atualContorno = GetP(yAtual, xAtual, width, height, stride, pixelSize, src);
                                }
                                else
                                {
                                    atualContorno = inicial; //forçar a saída do laço
                                }

                            } while(atualContorno != inicial); //enquanto verdade o contorno precisa continuar
                        }
                    }
                } //terminado esse laço eu tenho os meus contornos finalizados


                //escrever os marcados na imagem de destino
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (Marcado(src)) //preto no destino
                        {
                            *(dst++) = 0; //blue -> b
                            *(dst++) = 0; //green -> g
                            *(dst++) = 0; //red -> r
                        }
                        else //branco no destino
                        {
                            *(dst++) = 255; //blue -> b
                            *(dst++) = 255; //green -> g
                            *(dst++) = 255; //red -> r
                        }
                        src += pixelSize;
                    }
                    src += padding;
                    dst += padding;
                } //imagem destino com apenas os contornos
            }

            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }
    }
}
