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
        //nos meus métodos de GetPn, x é a linha e y é a coluna
        //alguns métodos para pegar os vizinhos de p1 -> para o zhang suen
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

        //metodo para auxiliar no algoritmo de zhang suen
        private static unsafe int Conectividade(byte* p2, byte* p3, byte* p4, byte* p5, byte* p6, byte* p7, byte* p8, byte* p9)
        {
            int v2, v3, v4, v5, v6, v7, v8, v9, conectividade;
            v2 = (p2 != null && (p2[0] + p2[1] + p2[2]) / 3 < 128) ? 1 : 0;
            v3 = (p3 != null && (p3[0] + p3[1] + p3[2]) / 3 < 128) ? 1 : 0;
            v4 = (p4 != null && (p4[0] + p4[1] + p4[2]) / 3 < 128) ? 1 : 0;
            v5 = (p5 != null && (p5[0] + p5[1] + p5[2]) / 3 < 128) ? 1 : 0;
            v6 = (p6 != null && (p6[0] + p6[1] + p6[2]) / 3 < 128) ? 1 : 0;
            v7 = (p7 != null && (p7[0] + p7[1] + p7[2]) / 3 < 128) ? 1 : 0;
            v8 = (p8 != null && (p8[0] + p8[1] + p8[2]) / 3 < 128) ? 1 : 0;
            v9 = (p9 != null && (p9[0] + p9[1] + p9[2]) / 3 < 128) ? 1 : 0;

            conectividade = 0;
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
            int v2, v3, v4, v5, v6, v7, v8, v9, pretos = 0;

            v2 = ((p2[0] + p2[1] + p2[2]) / 3 < 128) ? 1 : 0;
            v3 = ((p3[0] + p3[1] + p3[2]) / 3 < 128) ? 1 : 0;
            v5 = ((p5[0] + p5[1] + p5[2]) / 3 < 128) ? 1 : 0;
            v4 = ((p4[0] + p4[1] + p4[2]) / 3 < 128) ? 1 : 0;
            v6 = ((p6[0] + p6[1] + p6[2]) / 3 < 128) ? 1 : 0;
            v7 = ((p7[0] + p7[1] + p7[2]) / 3 < 128) ? 1 : 0;
            v8 = ((p8[0] + p8[1] + p8[2]) / 3 < 128) ? 1 : 0;
            v9 = ((p9[0] + p9[1] + p9[2]) / 3 < 128) ? 1 : 0;
            pretos = v2 + v3 + v4 + v5 + v6 + v7 + v8 + v9;
            return pretos;
        }

        private static unsafe int ContaFundo(byte* p1, byte* p2, byte* p3)
        {
            int fundo = 0, v2, v4, v8;
            v2 = ((p1[0] + p1[1] + p1[2]) / 3 < 128) ? 1 : 0;
            v4 = ((p2[0] + p2[1] + p2[2]) / 3 < 128) ? 1 : 0;
            v8 = ((p3[0] + p3[1] + p3[2]) / 3 < 128) ? 1 : 0;
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

        //BORDA CONTOUR FOLLOWING ----------------------------------------------------------------------------------------
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

        private static unsafe bool Conectividade4(Point pontoAtual, Point ponto)
        {
            if (ponto.X - 1 == pontoAtual.X && ponto.Y == pontoAtual.Y) //está na minha DIREITA
            {
                return true;
            }
            if(ponto.X + 1 == pontoAtual.X && ponto.Y == pontoAtual.Y) //está na minha ESQUERDA
            {
                return true;
            }
            if(ponto.X == pontoAtual.X && ponto.Y + 1 == pontoAtual.Y) //está ACIMA de mim
            {
                return true;
            }
            if(ponto.X == pontoAtual.X && ponto.Y - 1 == pontoAtual.Y) //está ABAIXO de mim
            {
                return true;
            }

            return false; //se chegou aqui o ponto escolhido não é conectividade 4 com o meu atual
        }

        private static unsafe void AcharPosicao(int xAtual, int yAtual, int width, int height, int stride, int pixelSize, byte* src, int direcao, List<Point> pixels, List<int> direcoes)
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
            byte*[] pixelsVet = new byte*[8] { p0, p1, p2, p3, p4, p5, p6, p7 };
            //bool achou = false;
            byte* pixelAtual = src + yAtual*stride + xAtual*pixelSize;

            for (int i=0; i<8; i++) //vou pegar todas as transições de branco para preto
            {
                int indice = (direcao + i + 1) % 8;

                if (pixelsVet[indice]!=null && IsPreto(pixelsVet[indice]))
                {
                    int indiceAnterior = (indice + 7) % 8;
                    if(pixelsVet[indiceAnterior]!=null && IsBranco(pixelsVet[indiceAnterior]))
                    {
                        Point ponto = new Point();
                        switch (indiceAnterior)
                        {
                            case 0: //direita
                                ponto.X = xAtual+1;
                                ponto.Y = yAtual;
                                break;
                            case 1: //acima direita
                                ponto.X = xAtual + 1;
                                ponto.Y = yAtual - 1;
                                break;
                            case 2: //acima
                                ponto.X = xAtual;
                                ponto.Y = yAtual - 1;
                                break;
                            case 3: //acima esquerda
                                ponto.X = xAtual - 1;
                                ponto.Y = yAtual - 1;
                                break;
                            case 4: //esquerda
                                ponto.X = xAtual - 1;
                                ponto.Y = yAtual;
                                break;
                            case 5: //abaixo esquerda
                                ponto.X = xAtual - 1;
                                ponto.Y = yAtual + 1;
                                break;
                            case 6: //abaixo
                                ponto.X = xAtual;
                                ponto.Y = yAtual + 1;
                                break;
                            case 7: //abaixo direita
                                ponto.X = xAtual + 1;
                                ponto.Y = yAtual + 1;
                                break;
                        }
                        Point pontoAtual = new Point();
                        pontoAtual.X = xAtual;
                        pontoAtual.Y = yAtual;
                        if (Conectividade4(pontoAtual, ponto))
                        {
                            direcoes.Insert(0, (indiceAnterior + 4) % 8);//enpilhar direcao
                            pixels.Insert(0, ponto); //empilhar ponto
                        }
                        else
                        {
                            int posAnt = (indiceAnterior + 7) % 8;
                            int posProx = (indiceAnterior + 1) % 8;
                            if ((pixelsVet[posAnt]!=null && IsBranco(pixelsVet[posAnt])) || (pixelsVet[posProx]!=null && IsBranco(pixelsVet[posProx]))) //verifico aqui se ele não passa nenhum limite de parede
                            {
                                direcoes.Insert(0, (indiceAnterior + 4) % 8);//enpilhar direcao
                                pixels.Insert(0, ponto); //empilhar ponto
                            }
                        }
                    }
                }
            }
        }

        public static unsafe void DesenharRetangulosDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest, List<Point> coordenadas)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3;

            //lock bits
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bitmapDataSrc.Stride;
            int padding = stride - (width * pixelSize);

            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer(); //ponteiro para o (0,0) de fonte
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer(); //ponteiro para o (0,0) de destino

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

                //fazer o retângulo
                dst = (byte*)bitmapDataDst.Scan0.ToPointer(); //ponteiro para o (0,0) de destino
                for (int i = 0; i < coordenadas.Count; i += 2)
                {
                    Point ponto1 = coordenadas[i];
                    Point ponto2 = coordenadas[i + 1];

                    int minX, minY, maxX, maxY;
                    maxX = ponto1.X;
                    maxY = ponto1.Y;
                    minX = ponto2.X;
                    minY = ponto2.Y;
                    for (int y2 = minY; y2 <= maxY; y2++)
                    {
                        byte* linhaRet = dst + y2 * stride + minX * pixelSize;
                        MarcarPixel(linhaRet);
                        linhaRet = dst + y2 * stride + maxX * pixelSize;
                        MarcarPixel(linhaRet);
                    }
                    for (int x2 = minX; x2 <= maxX; x2++)
                    {
                        byte* colunaRet = dst + minY * stride + x2 * pixelSize;
                        MarcarPixel(colunaRet);
                        colunaRet = dst + maxY * stride + x2 * pixelSize;
                        MarcarPixel(colunaRet);
                    }
                }
            }
            coordenadas.Clear();

            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }

        public static unsafe void ContornoDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest, List<Point> pontosRetangulo)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3;

            //lock bits
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bitmapDataSrc.Stride;
            int padding = stride - (width * pixelSize);

            List<int> direcoes = new List<int>(); //vai ser a minha pilha de direcoes
            List<Point> pixels = new List<Point>(); //vai ser a minha pilha de pixels

            int maxX, maxY, minX, minY;

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
                            maxX = maxY = Int32.MinValue;
                            minY = minX = Int32.MaxValue;
                            int direcao = 4;
                            do
                            {
                                MarcarPixel(atualContorno); //marcar o pixel atual como parte do contorno
                                AcharPosicao(xAtual, yAtual, width, height, stride, pixelSize, src, direcao, pixels, direcoes); //achar o próximo pixel do contorno
                                if (pixels.Count > 0) //tem informação na pilha
                                {
                                    Point ponto = pixels[0]; //desemplilhando
                                    direcao = direcoes[0]; //desempilhando
                                    pixels.RemoveAt(0); //excluindo da pilha
                                    direcoes.RemoveAt(0); //excluindo da pilha
                                    xAtual = ponto.X; //coluna
                                    yAtual = ponto.Y; //linha

                                    if(xAtual > maxX)
                                        maxX = xAtual;
                                    if(xAtual < minX)
                                        minX = xAtual;
                                    if(yAtual > maxY)
                                        maxY = yAtual;
                                    if(yAtual < minY)
                                        minY = yAtual;
                                    atualContorno = GetP(yAtual, xAtual, width, height, stride, pixelSize, src);
                                }
                                else
                                {
                                    atualContorno = inicial; //forçar a saída do laço
                                }

                            } while(atualContorno!=inicial); //enquanto verdade o contorno precisa continuar

                            //fazer o retângulo
                            Point ponto1 = new Point();
                            Point ponto2 = new Point();
                            ponto1.X = maxX;
                            ponto1.Y = maxY;
                            ponto2.X = minX;
                            ponto2.Y = minY;
                            pontosRetangulo.Add(ponto1);
                            pontosRetangulo.Add(ponto2);
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
