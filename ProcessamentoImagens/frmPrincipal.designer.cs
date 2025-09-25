namespace ProcessamentoImagens
{
    partial class frmPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictBoxImg1 = new System.Windows.Forms.PictureBox();
            this.pictBoxImg2 = new System.Windows.Forms.PictureBox();
            this.btnAbrirImagem = new System.Windows.Forms.Button();
            this.btnLimpar = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnLuminanciaSemDMA = new System.Windows.Forms.Button();
            this.btnLuminanciaComDMA = new System.Windows.Forms.Button();
            this.btnNegativoComDMA = new System.Windows.Forms.Button();
            this.btnNegativoSemDMA = new System.Windows.Forms.Button();
            this.btnEsqueletizarComDMA = new System.Windows.Forms.Button();
            this.btnBorda = new System.Windows.Forms.Button();
            this.espelhoHorizontal = new System.Windows.Forms.Button();
            this.espelhoVertical = new System.Windows.Forms.Button();
            this.canalAzul = new System.Windows.Forms.Button();
            this.canalVermelho = new System.Windows.Forms.Button();
            this.canalVerde = new System.Windows.Forms.Button();
            this.pretoBranco = new System.Windows.Forms.Button();
            this.noventa = new System.Windows.Forms.Button();
            this.inverteAzulVermelho = new System.Windows.Forms.Button();
            this.inverteDiagonal = new System.Windows.Forms.Button();
            this.rachaQuatro = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxImg1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxImg2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictBoxImg1
            // 
            this.pictBoxImg1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictBoxImg1.Location = new System.Drawing.Point(5, 6);
            this.pictBoxImg1.Name = "pictBoxImg1";
            this.pictBoxImg1.Size = new System.Drawing.Size(600, 500);
            this.pictBoxImg1.TabIndex = 102;
            this.pictBoxImg1.TabStop = false;
            // 
            // pictBoxImg2
            // 
            this.pictBoxImg2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictBoxImg2.Location = new System.Drawing.Point(611, 6);
            this.pictBoxImg2.Name = "pictBoxImg2";
            this.pictBoxImg2.Size = new System.Drawing.Size(600, 500);
            this.pictBoxImg2.TabIndex = 105;
            this.pictBoxImg2.TabStop = false;
            // 
            // btnAbrirImagem
            // 
            this.btnAbrirImagem.Location = new System.Drawing.Point(5, 512);
            this.btnAbrirImagem.Name = "btnAbrirImagem";
            this.btnAbrirImagem.Size = new System.Drawing.Size(101, 23);
            this.btnAbrirImagem.TabIndex = 106;
            this.btnAbrirImagem.Text = "Abrir Imagem";
            this.btnAbrirImagem.UseVisualStyleBackColor = true;
            this.btnAbrirImagem.Click += new System.EventHandler(this.btnAbrirImagem_Click);
            // 
            // btnLimpar
            // 
            this.btnLimpar.Location = new System.Drawing.Point(112, 512);
            this.btnLimpar.Name = "btnLimpar";
            this.btnLimpar.Size = new System.Drawing.Size(101, 23);
            this.btnLimpar.TabIndex = 107;
            this.btnLimpar.Text = "Limpar";
            this.btnLimpar.UseVisualStyleBackColor = true;
            this.btnLimpar.Click += new System.EventHandler(this.btnLimpar_Click);
            // 
            // btnLuminanciaSemDMA
            // 
            this.btnLuminanciaSemDMA.Location = new System.Drawing.Point(219, 512);
            this.btnLuminanciaSemDMA.Name = "btnLuminanciaSemDMA";
            this.btnLuminanciaSemDMA.Size = new System.Drawing.Size(125, 23);
            this.btnLuminanciaSemDMA.TabIndex = 108;
            this.btnLuminanciaSemDMA.Text = "Luminância sem DMA";
            this.btnLuminanciaSemDMA.UseVisualStyleBackColor = true;
            this.btnLuminanciaSemDMA.Click += new System.EventHandler(this.btnLuminanciaSemDMA_Click);
            // 
            // btnLuminanciaComDMA
            // 
            this.btnLuminanciaComDMA.Location = new System.Drawing.Point(219, 541);
            this.btnLuminanciaComDMA.Name = "btnLuminanciaComDMA";
            this.btnLuminanciaComDMA.Size = new System.Drawing.Size(125, 23);
            this.btnLuminanciaComDMA.TabIndex = 109;
            this.btnLuminanciaComDMA.Text = "Luminância com DMA";
            this.btnLuminanciaComDMA.UseVisualStyleBackColor = true;
            this.btnLuminanciaComDMA.Click += new System.EventHandler(this.btnLuminanciaComDMA_Click);
            // 
            // btnNegativoComDMA
            // 
            this.btnNegativoComDMA.Location = new System.Drawing.Point(350, 541);
            this.btnNegativoComDMA.Name = "btnNegativoComDMA";
            this.btnNegativoComDMA.Size = new System.Drawing.Size(125, 23);
            this.btnNegativoComDMA.TabIndex = 111;
            this.btnNegativoComDMA.Text = "Negativo com DMA";
            this.btnNegativoComDMA.UseVisualStyleBackColor = true;
            this.btnNegativoComDMA.Click += new System.EventHandler(this.btnNegativoComDMA_Click);
            // 
            // btnNegativoSemDMA
            // 
            this.btnNegativoSemDMA.Location = new System.Drawing.Point(350, 512);
            this.btnNegativoSemDMA.Name = "btnNegativoSemDMA";
            this.btnNegativoSemDMA.Size = new System.Drawing.Size(125, 23);
            this.btnNegativoSemDMA.TabIndex = 110;
            this.btnNegativoSemDMA.Text = "Negativo sem DMA";
            this.btnNegativoSemDMA.UseVisualStyleBackColor = true;
            this.btnNegativoSemDMA.Click += new System.EventHandler(this.btnNegativoSemDMA_Click);
            // 
            // btnEsqueletizarComDMA
            // 
            this.btnEsqueletizarComDMA.Location = new System.Drawing.Point(480, 512);
            this.btnEsqueletizarComDMA.Name = "btnEsqueletizarComDMA";
            this.btnEsqueletizarComDMA.Size = new System.Drawing.Size(125, 23);
            this.btnEsqueletizarComDMA.TabIndex = 112;
            this.btnEsqueletizarComDMA.Text = "Esqueletizar";
            this.btnEsqueletizarComDMA.UseVisualStyleBackColor = true;
            this.btnEsqueletizarComDMA.Click += new System.EventHandler(this.btnEsqueletizarComDMA_Click);
            // 
            // btnBorda
            // 
            this.btnBorda.Location = new System.Drawing.Point(480, 541);
            this.btnBorda.Name = "btnBorda";
            this.btnBorda.Size = new System.Drawing.Size(125, 23);
            this.btnBorda.TabIndex = 113;
            this.btnBorda.Text = "Borda e Retangulo";
            this.btnBorda.UseVisualStyleBackColor = true;
            this.btnBorda.Click += new System.EventHandler(this.btnBordaComDMA_Click);
            // 
            // espelhoHorizontal
            // 
            this.espelhoHorizontal.Location = new System.Drawing.Point(611, 512);
            this.espelhoHorizontal.Name = "espelhoHorizontal";
            this.espelhoHorizontal.Size = new System.Drawing.Size(125, 23);
            this.espelhoHorizontal.TabIndex = 114;
            this.espelhoHorizontal.Text = "Espelho Horizontal";
            this.espelhoHorizontal.UseVisualStyleBackColor = true;
            this.espelhoHorizontal.Click += new System.EventHandler(this.btnEspelhoHorizontal_Click);
            // 
            // espelhoVertical
            // 
            this.espelhoVertical.Location = new System.Drawing.Point(611, 541);
            this.espelhoVertical.Name = "espelhoVertical";
            this.espelhoVertical.Size = new System.Drawing.Size(125, 23);
            this.espelhoVertical.TabIndex = 115;
            this.espelhoVertical.Text = "EspelhoVertical";
            this.espelhoVertical.UseVisualStyleBackColor = true;
            this.espelhoVertical.Click += new System.EventHandler(this.btnEspelhoVertical_Click);
            // 
            // canalAzul
            // 
            this.canalAzul.Location = new System.Drawing.Point(742, 512);
            this.canalAzul.Name = "canalAzul";
            this.canalAzul.Size = new System.Drawing.Size(125, 23);
            this.canalAzul.TabIndex = 116;
            this.canalAzul.Text = "Canal Azul";
            this.canalAzul.UseVisualStyleBackColor = true;
            this.canalAzul.Click += new System.EventHandler(this.btnCanalAzul_Click);
            // 
            // canalVermelho
            // 
            this.canalVermelho.Location = new System.Drawing.Point(1004, 512);
            this.canalVermelho.Name = "canalVermelho";
            this.canalVermelho.Size = new System.Drawing.Size(125, 23);
            this.canalVermelho.TabIndex = 117;
            this.canalVermelho.Text = "Canal Vermelho";
            this.canalVermelho.UseVisualStyleBackColor = true;
            this.canalVermelho.Click += new System.EventHandler(this.btnCanalVermelho_Click);
            // 
            // canalVerde
            // 
            this.canalVerde.Location = new System.Drawing.Point(873, 512);
            this.canalVerde.Name = "canalVerde";
            this.canalVerde.Size = new System.Drawing.Size(125, 23);
            this.canalVerde.TabIndex = 118;
            this.canalVerde.Text = "Canal Verde";
            this.canalVerde.UseVisualStyleBackColor = true;
            this.canalVerde.Click += new System.EventHandler(this.btnCanalVerde_Click);
            // 
            // pretoBranco
            // 
            this.pretoBranco.Location = new System.Drawing.Point(742, 541);
            this.pretoBranco.Name = "pretoBranco";
            this.pretoBranco.Size = new System.Drawing.Size(125, 23);
            this.pretoBranco.TabIndex = 119;
            this.pretoBranco.Text = "Preto e Branco";
            this.pretoBranco.UseVisualStyleBackColor = true;
            this.pretoBranco.Click += new System.EventHandler(this.btnPretoBranco_Click);
            // 
            // noventa
            // 
            this.noventa.Location = new System.Drawing.Point(873, 541);
            this.noventa.Name = "noventa";
            this.noventa.Size = new System.Drawing.Size(125, 23);
            this.noventa.TabIndex = 120;
            this.noventa.Text = "Noventa";
            this.noventa.UseVisualStyleBackColor = true;
            this.noventa.Click += new System.EventHandler(this.btnNoventa_Click);
            // 
            // inverteAzulVermelho
            // 
            this.inverteAzulVermelho.Location = new System.Drawing.Point(1004, 541);
            this.inverteAzulVermelho.Name = "inverteAzulVermelho";
            this.inverteAzulVermelho.Size = new System.Drawing.Size(125, 23);
            this.inverteAzulVermelho.TabIndex = 121;
            this.inverteAzulVermelho.Text = "Inverte Azul Vermelho";
            this.inverteAzulVermelho.UseVisualStyleBackColor = true;
            this.inverteAzulVermelho.Click += new System.EventHandler(this.btnInverteAzulVermelho_Click);
            // 
            // inverteDiagonal
            // 
            this.inverteDiagonal.Location = new System.Drawing.Point(611, 570);
            this.inverteDiagonal.Name = "inverteDiagonal";
            this.inverteDiagonal.Size = new System.Drawing.Size(125, 23);
            this.inverteDiagonal.TabIndex = 122;
            this.inverteDiagonal.Text = "Inverte Diagonal";
            this.inverteDiagonal.UseVisualStyleBackColor = true;
            this.inverteDiagonal.Click += new System.EventHandler(this.btnInverteDiagonal_Click);
            // 
            // rachaQuatro
            // 
            this.rachaQuatro.Location = new System.Drawing.Point(742, 570);
            this.rachaQuatro.Name = "rachaQuatro";
            this.rachaQuatro.Size = new System.Drawing.Size(125, 23);
            this.rachaQuatro.TabIndex = 123;
            this.rachaQuatro.Text = "Racha 4";
            this.rachaQuatro.UseVisualStyleBackColor = true;
            this.rachaQuatro.Click += new System.EventHandler(this.rachaQuatro_Click);
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1220, 608);
            this.Controls.Add(this.rachaQuatro);
            this.Controls.Add(this.inverteDiagonal);
            this.Controls.Add(this.inverteAzulVermelho);
            this.Controls.Add(this.noventa);
            this.Controls.Add(this.pretoBranco);
            this.Controls.Add(this.canalVerde);
            this.Controls.Add(this.canalVermelho);
            this.Controls.Add(this.canalAzul);
            this.Controls.Add(this.espelhoVertical);
            this.Controls.Add(this.espelhoHorizontal);
            this.Controls.Add(this.btnBorda);
            this.Controls.Add(this.btnEsqueletizarComDMA);
            this.Controls.Add(this.btnNegativoComDMA);
            this.Controls.Add(this.btnNegativoSemDMA);
            this.Controls.Add(this.btnLuminanciaComDMA);
            this.Controls.Add(this.btnLuminanciaSemDMA);
            this.Controls.Add(this.btnLimpar);
            this.Controls.Add(this.btnAbrirImagem);
            this.Controls.Add(this.pictBoxImg2);
            this.Controls.Add(this.pictBoxImg1);
            this.Name = "frmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Formulário Principal";
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxImg1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxImg2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictBoxImg1;
        private System.Windows.Forms.PictureBox pictBoxImg2;
        private System.Windows.Forms.Button btnAbrirImagem;
        private System.Windows.Forms.Button btnLimpar;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnLuminanciaSemDMA;
        private System.Windows.Forms.Button btnLuminanciaComDMA;
        private System.Windows.Forms.Button btnNegativoComDMA;
        private System.Windows.Forms.Button btnNegativoSemDMA;
        private System.Windows.Forms.Button btnEsqueletizarComDMA;
        private System.Windows.Forms.Button btnBorda;
        private System.Windows.Forms.Button espelhoHorizontal;
        private System.Windows.Forms.Button espelhoVertical;
        private System.Windows.Forms.Button canalAzul;
        private System.Windows.Forms.Button canalVermelho;
        private System.Windows.Forms.Button canalVerde;
        private System.Windows.Forms.Button pretoBranco;
        private System.Windows.Forms.Button noventa;
        private System.Windows.Forms.Button inverteAzulVermelho;
        private System.Windows.Forms.Button inverteDiagonal;
        private System.Windows.Forms.Button rachaQuatro;
    }
}

