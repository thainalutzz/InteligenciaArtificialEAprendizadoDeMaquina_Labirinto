namespace A2_Descricao2
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
     
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MazeButton = new System.Windows.Forms.Button();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ClearButton = new System.Windows.Forms.Button();
            this.dfs = new System.Windows.Forms.RadioButton();
            this.guloso = new System.Windows.Forms.RadioButton();
            this.BotaoAnimacao = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mensagem = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BotaoLabirinto
            // 
            this.MazeButton.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.MazeButton.Location = new System.Drawing.Point(520, 136);
            this.MazeButton.Name = "MazeButton";
            this.MazeButton.Size = new System.Drawing.Size(168, 28);
            this.MazeButton.TabIndex = 6;
            this.MazeButton.Text = "Gerar Labirinto";
            this.ToolTip.SetToolTip(this.MazeButton, "Creates a random maze");
            this.MazeButton.UseVisualStyleBackColor = true;
            this.MazeButton.Click += new System.EventHandler(this.BotaoLabirintoAcao);
            // 
            // BotaoLimpar
            // 
            this.ClearButton.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ClearButton.Location = new System.Drawing.Point(520, 204);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(168, 28);
            this.ClearButton.TabIndex = 7;
            this.ClearButton.Text = "Limpar";
            this.ToolTip.SetToolTip(this.ClearButton, "First click: clears search, Second click: clears obstacles");
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.BotaoLimparAcao);
            // 
            // Busca por profundidade
            // 
            this.dfs.AutoSize = true;
            this.dfs.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.dfs.Location = new System.Drawing.Point(16, 24);
            this.dfs.Name = "dfs";
            this.dfs.Size = new System.Drawing.Size(143, 20);
            this.dfs.TabIndex = 0;
            this.dfs.TabStop = true;
            this.dfs.Text = "Depth First Search";
            this.ToolTip.SetToolTip(this.dfs, "Depth First Search algorithm");
            this.dfs.UseVisualStyleBackColor = true;
            // 
            // guloso
            // 
            this.guloso.AutoSize = true;
            this.guloso.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.guloso.Location = new System.Drawing.Point(16, 50);
            this.guloso.Name = "guloso";
            this.guloso.Size = new System.Drawing.Size(124, 20);
            this.guloso.TabIndex = 3;
            this.guloso.TabStop = true;
            this.guloso.Text = "Guloso(Greedy)";
            this.ToolTip.SetToolTip(this.guloso, "Greedy search algorithm");
            this.guloso.UseVisualStyleBackColor = true;
            // 
            // BotaoAnimacao
            // 
            this.BotaoAnimacao.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.BotaoAnimacao.Location = new System.Drawing.Point(520, 170);
            this.BotaoAnimacao.Name = "BotaoAnimacao";
            this.BotaoAnimacao.Size = new System.Drawing.Size(168, 28);
            this.BotaoAnimacao.TabIndex = 21;
            this.BotaoAnimacao.Text = "Realizar Busca ";
            this.ToolTip.SetToolTip(this.BotaoAnimacao, "Position of obstacles, robot and target can be changed when search is underway");
            this.BotaoAnimacao.UseVisualStyleBackColor = true;
            this.BotaoAnimacao.Click += new System.EventHandler(this.BotaoAnimacaoAcao);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.guloso);
            this.groupBox1.Controls.Add(this.dfs);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.groupBox1.Location = new System.Drawing.Point(520, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(168, 90);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Algoritimos";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // mensagem
            // 
            this.mensagem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mensagem.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.mensagem.ForeColor = System.Drawing.Color.Blue;
            this.mensagem.Location = new System.Drawing.Point(516, 324);
            this.mensagem.MinimumSize = new System.Drawing.Size(100, 100);
            this.mensagem.Name = "mensagem";
            this.mensagem.Size = new System.Drawing.Size(170, 200);
            this.mensagem.TabIndex = 20;
            this.mensagem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.mensagem.Click += new System.EventHandler(this.CaixaMensagem);
            // 
            // timer
            // 
            this.timer.Interval = 60;
            this.timer.Tick += new System.EventHandler(this.Marca_Timer);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 547);
            this.Controls.Add(this.BotaoAnimacao);
            this.Controls.Add(this.mensagem);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.MazeButton);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "A2";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button MazeButton;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton dfs;
        private System.Windows.Forms.RadioButton guloso;
        private System.Windows.Forms.Label mensagem;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button BotaoAnimacao;
    }
}

