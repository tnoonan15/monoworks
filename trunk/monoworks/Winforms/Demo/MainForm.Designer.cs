namespace Demo
{
	partial class MainForm
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
			this._viewportAdapter = new MonoWorks.Swf.Backend.ViewportAdapter();
			this.SuspendLayout();
			// 
			// _viewportAdapter
			// 
			this._viewportAdapter.AccumBits = ((byte)(0));
			this._viewportAdapter.AutoCheckErrors = false;
			this._viewportAdapter.AutoFinish = false;
			this._viewportAdapter.AutoMakeCurrent = true;
			this._viewportAdapter.AutoSwapBuffers = true;
			this._viewportAdapter.BackColor = System.Drawing.Color.Black;
			this._viewportAdapter.ColorBits = ((byte)(32));
			this._viewportAdapter.DepthBits = ((byte)(16));
			this._viewportAdapter.Dock = System.Windows.Forms.DockStyle.Fill;
			this._viewportAdapter.Location = new System.Drawing.Point(0, 0);
			this._viewportAdapter.Name = "_viewportAdapter";
			this._viewportAdapter.Size = new System.Drawing.Size(1024, 571);
			this._viewportAdapter.StencilBits = ((byte)(0));
			this._viewportAdapter.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1024, 571);
			this.Controls.Add(this._viewportAdapter);
			this.Name = "MainForm";
			this.Text = "MonoWorks Demo";
			this.ResumeLayout(false);

		}

		#endregion

		private MonoWorks.Swf.Backend.ViewportAdapter _viewportAdapter;
	}
}

