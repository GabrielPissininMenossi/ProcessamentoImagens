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

        //BORDA COUNTING FOLLOWING ----------------------------------------------------------------------------------------
        private static unsafe Boolean DireitaIsPreto(byte* pixelAtual)
        {
            byte* direita = pixelAtual + 3;
            return IsPreto(direita);
        }

        private static unsafe byte* AcharVizinho(int dirAnt, byte* p0, byte* p1, byte* p2, byte* p3, byte* p4, byte* p5, byte* p6, byte* p7)
        {
            // começar busca no vizinho seguinte ao anterior (anti-horário)
            int inicio = (dirAnt + 1) % 8;
            byte* p = null;
            Boolean flag = false;
            int pos;

            for (int i = 0; i < 8 && !flag; i++)
            {
                pos = (inicio + i) % 8;
                switch (pos)
                {
                    case 0:
                        p = p0;
                        break;
                    case 1:
                        p = p1;
                        break;
                    case 2:
                        p = p2;
                        break;
                    case 3:
                        p = p3;
                        break;
                    case 4:
                        p = p4;
                        break;
                    case 5:
                        p = p5;
                        break;
                    case 6:
                        p = p6;
                        break;
                    case 7:
                        p = p7;
                        break;
                }

                if (p != null && !Marcado(p) && IsPreto(p))
                {
                    flag = true;
                    //return p;
                }
            }

            if (flag) //então achei o próximo vizinho
                return p;
            return null;
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

        private static unsafe Boolean IsPreto(byte* pixel)
        {
            return pixel[0] == 0 && pixel[1] == 0 && pixel[2] == 0;
        }

        private static unsafe Boolean IsBranco(byte* pixel)
        {
            return pixel[0] == 255 && pixel[1] == 255 && pixel[2] == 255;
        }

        private static unsafe int IndiceDoVizinho(byte* prox, byte* p0, byte* p1, byte* p2, byte* p3, byte* p4, byte* p5, byte* p6, byte* p7)
        {
            if (prox == p0) return 0;
            if (prox == p1) return 1;
            if (prox == p2) return 2;
            if (prox == p3) return 3;
            if (prox == p4) return 4;
            if (prox == p5) return 5;
            if (prox == p6) return 6;
            if (prox == p7) return 7;
            return -1; //só para não acusar erro
        }

        public static void BordaDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width; // pega a largura da imagem em inteiros
            int height = imageBitmapSrc.Height; // pega a altura da imagem em inteiros
            int pixelSize = 3;

            //lock dados bitmap origem 
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //lock dados bitmap destino
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {

                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();
                byte* srcAux, dstAux, aux, aux1;

                //deixar a imagem de destino zerada
                for (int y = 0; y < imageBitmapDest.Height; y++)
                {
                    for (int x = 0; x < imageBitmapDest.Width; x++)
                    {
                        aux1 = dst + y * bitmapDataDst.Stride + x * pixelSize;
                        aux1[0] = 255;
                        aux1[1] = 255;
                        aux1[2] = 255;
                    }
                }

                int pR, r0 = -1, r1 = -1, r2 = -1, r3 = -1, r4 = -1, r5 = -1, r6 = -1, r7 = -1;
                int pG, g0 = -1, g1 = -1, g2 = -1, g3 = -1, g4 = -1, g5 = -1, g6 = -1, g7 = -1;
                int pB, b0 = -1, b1 = -1, b2 = -1, b3 = -1, b4 = -1, b5 = -1, b6 = -1, b7 = -1;
                byte* p0, p1, p2, p3, p4, p5, p6, p7;
                int direcao, proximaDirecao;
                int cordX, cordY, inicioY, inicioX, i, flag;
                int[] coordenadas = new int[(width * height) * 2];
                int TL = 0;
                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        srcAux = src + y * bitmapDataSrc.Stride + x * pixelSize; // desloco o ponteiro para meu y e x atual
                        pB = *(srcAux++);
                        pG = *(srcAux++);
                        pR = *(srcAux++);

                        aux = src + y * bitmapDataSrc.Stride + (x + 1) * pixelSize; // pixel à direita
                        b0 = (*aux++);
                        g0 = (*aux++);
                        r0 = (*aux++);
                        if ((pR >= 127 && pG >= 127 && pB >= 127) && (r0 < 127 && g0 < 127 && b0 < 127)) // pixel preto à direita do branco
                        {
                            // primeiro elemento do contorno  
                            direcao = 4;

                            inicioY = y;
                            inicioX = x + 1;
                            cordX = inicioX;
                            cordY = inicioY;
                            coordenadas[TL++] = cordY; // linha
                            coordenadas[TL++] = cordX; // coluna
                            do
                            {

                                //aqui eu pego os meus vizinhos
                                if (cordX + 1 < width) // validação para não acessar lixo
                                {
                                    aux = src + cordY * bitmapDataSrc.Stride + (cordX + 1) * pixelSize;
                                    b0 = (*aux++);
                                    g0 = (*aux++);
                                    r0 = (*aux++);
                                }

                                if (cordX + 1 < width && cordY - 1 >= 0)
                                {
                                    aux = src + (cordY - 1) * bitmapDataSrc.Stride + (cordX + 1) * pixelSize;
                                    b1 = (*aux++);
                                    g1 = (*aux++);
                                    r1 = (*aux++);
                                }

                                if (cordY - 1 >= 0)
                                {
                                    aux = src + (cordY - 1) * bitmapDataSrc.Stride + cordX * pixelSize;
                                    b2 = (*aux++);
                                    g2 = (*aux++);
                                    r2 = (*aux++);
                                }

                                if (cordY - 1 >= 0 && cordX - 1 >= 0)
                                {
                                    aux = src + (cordY - 1) * bitmapDataSrc.Stride + (cordX - 1) * pixelSize;
                                    b3 = (*aux++);
                                    g3 = (*aux++);
                                    r3 = (*aux++);
                                }

                                if (cordX - 1 >= 0)
                                {
                                    aux = src + cordY * bitmapDataSrc.Stride + (cordX - 1) * pixelSize;
                                    b4 = (*aux++);
                                    g4 = (*aux++);
                                    r4 = (*aux++);
                                }

                                if (cordY + 1 < height && cordX - 1 >= 0)
                                {
                                    aux = src + (cordY + 1) * bitmapDataSrc.Stride + (cordX - 1) * pixelSize;
                                    b5 = (*aux++);
                                    g5 = (*aux++);
                                    r5 = (*aux++);
                                }

                                if (cordY + 1 < height)
                                {
                                    aux = src + (cordY + 1) * bitmapDataSrc.Stride + cordX * pixelSize;
                                    b6 = (*aux++);
                                    g6 = (*aux++);
                                    r6 = (*aux++);
                                }

                                if (cordY + 1 < height && cordX + 1 < width)
                                {
                                    aux = src + (cordY + 1) * bitmapDataSrc.Stride + (cordX + 1) * pixelSize;
                                    b7 = (*aux++);
                                    g7 = (*aux++);
                                    r7 = (*aux++);
                                }

                                i = 0;
                                flag = 0;
                                while (i < 8 && flag != 1) // proximo p
                                {
                                    proximaDirecao = (direcao + 1 + i) % 8; // da onde eu vim

                                    if (proximaDirecao == 0 && cordX + 1 < width && r0 < 127 && g0 < 127 && b0 < 127)
                                    {
                                        cordX++;
                                        flag = 1;
                                    }
                                    else
                                    if (proximaDirecao == 1 && cordX + 1 < width && cordY - 1 >= 0 && r1 < 127 && g1 < 127 && b1 < 127)
                                    {
                                        cordY--;
                                        cordX++;
                                        flag = 1;
                                    }
                                    else
                                    if (proximaDirecao == 2 && cordY - 1 >= 0 && r2 < 127 && g2 < 127 && b2 < 127)
                                    {
                                        cordY--;
                                        flag = 1;
                                    }
                                    else
                                    if (proximaDirecao == 3 && cordY - 1 >= 0 && r3 < 127 && g3 < 127 && b3 < 127)
                                    {
                                        cordY--;
                                        cordX--;
                                        flag = 1;
                                    }
                                    else
                                    if (proximaDirecao == 4 && cordX - 1 >= 0 && r4 < 127 && g4 < 127 && b4 < 127)
                                    {
                                        cordX--;
                                        flag = 1;
                                    }
                                    else
                                    if (proximaDirecao == 5 && cordY + 1 < height && cordX - 1 >= 0 && r5 < 127 && g5 < 127 && b5 < 127)
                                    {

                                        cordY++;
                                        cordX--;
                                        flag = 1;
                                    }
                                    else
                                    if (proximaDirecao == 6 && cordY + 1 < height && r6 < 127 && g6 < 127 && b6 < 127)
                                    {
                                        cordY++;
                                        flag = 1;
                                    }
                                    else
                                    if (proximaDirecao == 7 && cordX + 1 < width && cordY + 1 < height && r7 < 127 && g7 < 127 && b7 < 127)
                                    {

                                        cordY++;
                                        cordX++;
                                        flag = 1;

                                    }

                                    if (flag == 1)
                                    {
                                        direcao = (proximaDirecao + 4) % 8;
                                        coordenadas[TL++] = cordY; // linha
                                        coordenadas[TL++] = cordX; // coluna
                                    }
                                    i++;
                                }

                            } while (cordX != inicioX || cordY != inicioY);
                        }

                    }
                }

                i = 0;
                while (i < TL)
                {
                    cordY = coordenadas[i++];
                    cordX = coordenadas[i++];

                    if (cordX + 1 < width)
                    {
                        aux = src + cordY * bitmapDataSrc.Stride + (cordX + 1) * pixelSize;
                        b0 = (*aux++);
                        g0 = (*aux++);
                        r0 = (*aux++);
                    }

                    if (cordX + 1 < width && cordY - 1 >= 0)
                    {
                        aux = src + (cordY - 1) * bitmapDataSrc.Stride + (cordX + 1) * pixelSize;
                        b1 = (*aux++);
                        g1 = (*aux++);
                        r1 = (*aux++);
                    }

                    if (cordY - 1 >= 0)
                    {
                        aux = src + (cordY - 1) * bitmapDataSrc.Stride + cordX * pixelSize;
                        b2 = (*aux++);
                        g2 = (*aux++);
                        r2 = (*aux++);
                    }

                    if (cordY - 1 >= 0 && cordX - 1 >= 0)
                    {
                        aux = src + (cordY - 1) * bitmapDataSrc.Stride + (cordX - 1) * pixelSize;
                        b3 = (*aux++);
                        g3 = (*aux++);
                        r3 = (*aux++);
                    }

                    if (cordX - 1 >= 0)
                    {
                        aux = src + cordY * bitmapDataSrc.Stride + (cordX - 1) * pixelSize;
                        b4 = (*aux++);
                        g4 = (*aux++);
                        r4 = (*aux++);
                    }

                    if (cordY + 1 < height && cordX - 1 >= 0)
                    {
                        aux = src + (cordY + 1) * bitmapDataSrc.Stride + (cordX - 1) * pixelSize;
                        b5 = (*aux++);
                        g5 = (*aux++);
                        r5 = (*aux++);
                    }

                    if (cordY + 1 < height)
                    {
                        aux = src + (cordY + 1) * bitmapDataSrc.Stride + cordX * pixelSize;
                        b6 = (*aux++);
                        g6 = (*aux++);
                        r6 = (*aux++);
                    }

                    if (cordY + 1 < height && cordX + 1 < width)
                    {
                        aux = src + (cordY + 1) * bitmapDataSrc.Stride + (cordX + 1) * pixelSize;
                        b7 = (*aux++);
                        g7 = (*aux++);
                        r7 = (*aux++);
                    }

                    if (r0 >= 127 && g0 >= 127 && b0 >= 127)
                    {
                        dstAux = dst + cordY * bitmapDataDst.Stride + (cordX + 1) * pixelSize;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                    }
                    if (r1 >= 127 && g1 >= 127 && b1 >= 127)
                    {
                        dstAux = dst + (cordY - 1) * bitmapDataDst.Stride + (cordX + 1) * pixelSize;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                    }
                    if (r2 >= 127 && g2 >= 127 && b2 >= 127)
                    {
                        dstAux = dst + (cordY - 1) * bitmapDataDst.Stride + cordX * pixelSize;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                    }
                    if (r3 >= 127 && g3 >= 127 && b3 >= 127)
                    {
                        dstAux = dst + (cordY - 1) * bitmapDataDst.Stride + (cordX - 1) * pixelSize;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                    }
                    if (r4 >= 127 && g4 >= 127 && b4 >= 127)
                    {
                        dstAux = dst + cordY * bitmapDataDst.Stride + (cordX - 1) * pixelSize;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                    }
                    if (r5 >= 127 && g5 >= 127 && b5 >= 127)
                    {
                        dstAux = dst + (cordY + 1) * bitmapDataDst.Stride + (cordX - 1) * pixelSize;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                    }
                    if (r6 >= 127 && g6 >= 127 && b6 >= 127)
                    {
                        dstAux = dst + (cordY + 1) * bitmapDataDst.Stride + cordX * pixelSize;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                    }
                    if (r7 >= 127 && g7 >= 127 && b7 >= 127)
                    {
                        dstAux = dst + (cordY + 1) * bitmapDataDst.Stride + (cordX + 1) * pixelSize;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                        *(dstAux++) = 0;
                    }
                }
            }

            //unlock imagem origem
            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            //unlock imagem destino
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }
    }
}
