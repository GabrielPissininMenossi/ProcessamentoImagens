using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.NetworkInformation;

namespace ProcessamentoImagens
{
    class Filtros
    {
        //variaveis para os pixels vizinhos do shang suen
        private unsafe byte* p1, p2, p3, p4, p5, p6, p7, p8, p9;

        //

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
        } //feito

        private static unsafe byte* GetP3(int x, int y, int width, int height, int pixelSize, int stride, byte* basePixel)
        {
            if (y + 1 < width && x - 1 >= 0) //verifica o range de coluna e linha
            {
                //pixel acima a direita
                byte* p = basePixel + ((x - 1) * stride) + ((y + 1) * pixelSize);

                return p;
            }
            return null;
        } //feito
        private static unsafe byte* GetP4(int x, int y, int width, int height, int pixelSize, int stride, byte* basePixel)
        {
            if (y + 1 < width)
            {
                //pixel à direita
                byte* p = basePixel + (x * stride) + ((y + 1) * pixelSize);

                return p;
            }
            return null;
        } //feito
        private static unsafe byte* GetP5(int x, int y, int width, int height, int pixelSize, int stride, byte* basePixel)
        {
            if (y + 1 < width && x + 1 <= height) //verificar o range de baixo
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

                        //Color pixel = imageBitmapSrc.GetPixel(y, x);
                        int media = (aux1[0] + aux1[1] + aux1[2]) / 3;
                        if (media < 128)
                        {
                            aux1[0] = 0;
                            aux1[1] = 0;
                            aux2[0] = 0;
                            aux1[2] = 0;
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
                        //imageBitmapSrc.SetPixel(y, x, novo);
                        //imageBitmapDest.SetPixel(y, x, novo);
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

        private static unsafe Boolean DireitaIsPreto(byte* pixelAtual)
        {
            byte* direita = pixelAtual + 3;
            if (direita[0]==0 && direita[1]==0 && direita[2]==0)
                return true;
            return false;
        }

        private static unsafe int AcharVizinho(byte* p0, byte* p1, byte* p2, byte* p3, byte* p4, byte* p5, byte* p6, byte* p7)
        {
            if (p0 != null && !Marcado(p0) && p0[0] * p0[1] * p0[2] == 0)
                return 0;
            if (p1 != null && !Marcado(p1) && p1[0] * p1[1] * p1[2] == 0)
                return 1;
            if (p2 != null && !Marcado(p2) && p2[0] * p2[1] * p2[2] == 0)
                return 2;
            if (p3 != null && !Marcado(p3) && p3[0] * p3[1] * p3[2] == 0)
                return 3;
            if (p4 != null && !Marcado(p4) && p4[0] * p4[1] * p4[2] == 0)
                return 4;
            if (p5 != null && !Marcado(p5) && p5[0] * p5[1] * p5[2] == 0)
                return 5;
            if (p6 != null && !Marcado(p6) && p6[0] * p6[1] * p6[2] == 0)
                return 6;
            if (p7 != null && !Marcado(p7) && p7[0] * p7[1] * p7[2] == 0)
                return 7;
            return -1;
        }

        private static unsafe void MarcarPixel(byte* pixel)
        {
            //o meu pixel ficará marcado como vermelho
            pixel[0] = 0; //b
            pixel[1] = 0; //g
            pixel[2] = 255; //r
        }

        private static unsafe Boolean Marcado(byte* pixel)
        {
            //retorna true se o pixel é inteiramente vermelho
            return pixel[0] == 0 && pixel[1] == 0 && pixel[2] == 255;
        }

        public static void BordaDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int numVizinho;
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int[] x1, x2, y1, y2; //vão ser os vetores para guardar as coordenadas dos retangulos
            x1 = new int[width * height];
            x2 = new int[width * height];
            y1 = new int[width * height];
            y2 = new int[width * height];

            //trava a região de memória
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {
                //definindo os ponteiros para o inicio de cada imagem
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();
                byte* p0, p1, p2, p3, p4, p5, p6, p7, auxSrc, auxDst;

                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        auxSrc = src + x * bitmapDataSrc.Stride + y * 3;
                        if (!Marcado(auxSrc) && DireitaIsPreto(auxSrc)) //se verdade, então aqui eu começo de fato a traçar a borda
                        {
                            p0 = GetP4(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel à direita
                            p1 = GetP3(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel acima à direita
                            p2 = GetP2(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel acima
                            p3 = GetP9(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel acima à esquerda
                            p4 = GetP8(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel à esquerda
                            p5 = GetP7(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel abaixo à esquerda
                            p6 = GetP6(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel abaixo
                            p7 = GetP5(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel abaixo à direita

                            numVizinho = AcharVizinho(p0, p1, p2, p3, p4, p5, p6, p7);
                            while(numVizinho > -1)// posso continuar o algoritmo pois achei um vizinho para andar
                            {
                                //marca o pixel atual como que já passei por ele
                                MarcarPixel(auxSrc);

                                //vai para o vizinho
                                switch (numVizinho)
                                {
                                    case 0:
                                        {
                                            auxSrc = p0;
                                            break;
                                        }
                                    case 1:
                                        {
                                            auxSrc = p1;
                                            break;
                                        }
                                    case 2:
                                        {
                                            auxSrc = p2;
                                            break;
                                        }
                                    case 3:
                                        {
                                            auxSrc = p3;
                                            break;
                                        }
                                    case 4:
                                        {
                                            auxSrc = p4;
                                            break;
                                        }
                                    case 5:
                                        {
                                            auxSrc = p5;
                                            break;
                                        }
                                    case 6:
                                        {
                                            auxSrc = p6;
                                            break;
                                        }
                                    case 7:
                                        {
                                            auxSrc = p7;
                                            break;
                                        }
                                }

                                p0 = GetP4(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel à direita
                                p1 = GetP3(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel acima à direita
                                p2 = GetP2(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel acima
                                p3 = GetP9(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel acima à esquerda
                                p4 = GetP8(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel à esquerda
                                p5 = GetP7(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel abaixo à esquerda
                                p6 = GetP6(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel abaixo
                                p7 = GetP5(x, y, width, height, 3, bitmapDataSrc.Stride, src); //pixel abaixo à direita
                                numVizinho = AcharVizinho(p0, p1, p2, p3, p4, p5, p6, p7);
                            }
                        }
                    }
                }

                //os pixels que não foram marcados recebem branco, e os marcados agora serão pretos (borda)
                for(int x=0; x<height; x++)
                {
                    for(int y=0; y<width; y++)
                    {
                        auxSrc = src + x * bitmapDataSrc.Stride + y * 3;
                        if (!Marcado(auxSrc))
                        {
                            auxSrc[0] = 255;
                            auxSrc[1] = 255;
                            auxSrc[2] = 255;
                        }
                        else
                        {
                            auxSrc[0] = 0;
                            auxSrc[1] = 0;
                            auxSrc[2] = 0;
                        }
                    }
                }
            }

            //destrava a região de memória
            //unlock imagem origem
            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            //unlock imagem destino
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }
    }
}
