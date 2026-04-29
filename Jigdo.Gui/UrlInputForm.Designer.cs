namespace Jigdo.Gui
{
    partial class UrlInputForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UrlInputForm));
            buttonGo = new Button();
            label1 = new Label();
            textBoxUrl = new TextBox();
            SuspendLayout();
            // 
            // buttonGo
            // 
            buttonGo.Location = new Point(557, 56);
            buttonGo.Name = "buttonGo";
            buttonGo.Size = new Size(75, 23);
            buttonGo.TabIndex = 0;
            buttonGo.Text = "Go";
            buttonGo.UseVisualStyleBackColor = true;
            buttonGo.Click += buttonGo_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 26);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 1;
            label1.Text = "URL:";
            // 
            // textBoxUrl
            // 
            textBoxUrl.Location = new Point(49, 23);
            textBoxUrl.Name = "textBoxUrl";
            textBoxUrl.Size = new Size(583, 23);
            textBoxUrl.TabIndex = 2;
            // 
            // UrlInputForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(644, 91);
            Controls.Add(textBoxUrl);
            Controls.Add(label1);
            Controls.Add(buttonGo);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(660, 130);
            MinimizeBox = false;
            Name = "UrlInputForm";
            Text = "URL Input";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonGo;
        private Label label1;
        private TextBox textBoxUrl;
    }
}