using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProgressDisk
{
	[DefaultProperty("BlockSize")]
	public partial class ProgressDisk : UserControl
	{
		private GraphicsPath bkGroundPath1 = new GraphicsPath();
		private GraphicsPath bkGroundPath2 = new GraphicsPath();
		private GraphicsPath valuePath = new GraphicsPath();
		private GraphicsPath freGroundPath = new GraphicsPath();
		private Region region = new Region();

		private Color inactiveforeColor2 = Color.LightGreen;
		private Color backGrndColor = Color.White;
		private Color activeforeColor1 = Color.Blue;
		private Color activeforeColor2 = Color.LightBlue;
		private Color inactiveforeColor1 = Color.Green;

		private int sliceCount;
		private int value;
		private int size = 50;
		private float blockRatio = .4f;
		private BlockSize bs = BlockSize.Small;

		public ProgressDisk()
		{
			InitializeComponent();
			// CheckForIllegalCrossThreadCalls = false;
			Render();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			region = new Region(ClientRectangle);
			if (backGrndColor == Color.Transparent)
			{
				region.Exclude(bkGroundPath2);
				Region = region;
			}

			e.Graphics.FillPath(new SolidBrush(backGrndColor), bkGroundPath1);
			e.Graphics.FillPath(
			new LinearGradientBrush(new Rectangle(0, 0, size, size), inactiveforeColor1, inactiveforeColor2,
			value * 360 / 12, true), valuePath);
			e.Graphics.FillPath(
			new LinearGradientBrush(new Rectangle(0, 0, size, size), activeforeColor1, activeforeColor2,
			value * 360 / 12, true), freGroundPath);
			e.Graphics.FillPath(new SolidBrush(backGrndColor), bkGroundPath2);

			base.OnPaint(e);
		}

		private void Render()
		{
			// bkGroundPath1 = new GraphicsPath();
			// bkGroundPath2 = new GraphicsPath();
			// valuePath = new GraphicsPath();
			// freGroundPath = new GraphicsPath();
			bkGroundPath1.Reset();
			bkGroundPath2.Reset();
			valuePath.Reset();
			freGroundPath.Reset();
			bkGroundPath1.AddPie(new Rectangle(0, 0, size, size), 0, 360);

			//just in case...
			if (sliceCount == 0)
			{
				sliceCount = 12;
			}

			float sliceAngle = 360 / sliceCount;
			float sweepAngle = sliceAngle - 5;
			for (int i = 0; i < sliceCount; i++)
			{
				if (value != i)
				{
					valuePath.AddPie(0, 0, size, size, i * sliceAngle, sweepAngle);
				}
			}
			bkGroundPath2.AddPie(
			(size / 2 - size * blockRatio), (size / 2 - size * blockRatio),
			(blockRatio * 2 * size), (blockRatio * 2 * size), 0, 360);
			freGroundPath.AddPie(new Rectangle(0, 0, size, size), value * sliceAngle, sweepAngle);
			Invalidate();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			size = Math.Max(Width, Height);
			Size = new Size(size, size);
			Render();
			base.OnSizeChanged(e);
		}

		protected override void OnResize(EventArgs e)
		{
			size = Math.Max(Width, Height);
			Size = new Size(size, size);
			Render();
			base.OnResize(e);
		}

		#region Properties
		[DefaultValue(0)]
		public int Value
		{
			get { return value; }
			set
			{
				this.value = value;
				Render();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		public Color BackGroundColor
		{
			get { return backGrndColor; }
			set
			{
				backGrndColor = value;
				Render();
			}
		}

		[DefaultValue(typeof(Color), "Blue")]
		public Color ActiveForeColor1
		{
			get { return activeforeColor1; }
			set
			{
				activeforeColor1 = value;
				Render();
			}
		}

		[DefaultValue(typeof(Color), "LightBlue")]
		public Color ActiveForeColor2
		{
			get { return activeforeColor2; }
			set
			{
				activeforeColor2 = value;
				Render();
			}
		}

		[DefaultValue(typeof(Color), "Green")]
		public Color InactiveForeColor1
		{
			get { return inactiveforeColor1; }
			set
			{
				inactiveforeColor1 = value;
				Render();
			}
		}

		[DefaultValue(typeof(Color), "LightGreen")]
		public Color InactiveForeColor2
		{
			get { return inactiveforeColor2; }
			set
			{
				inactiveforeColor2 = value;
				Render();
			}
		}

		[DefaultValue(50)]
		public int SquareSize
		{
			get { return size; }
			set
			{
				size = value;
				Size = new Size(size, size);
			}
		}

		[DefaultValue(typeof(BlockSize), "Small")]
		public BlockSize BlockSize
		{
			get { return bs; }
			set
			{
				bs = value;
				switch (bs)
				{
					case BlockSize.XSmall:
						blockRatio = 0.49f;
						break;
					case BlockSize.Small:
						blockRatio = 0.4f;
						break;
					case BlockSize.Medium:
						blockRatio = 0.3f;
						break;
					case BlockSize.Large:
						blockRatio = 0.2f;
						break;
					case BlockSize.XLarge:
						blockRatio = 0.1f;
						break;
					case BlockSize.XXLarge:
						blockRatio = 0.01f;
						break;
					default:
						break;
				}
			}
		}

		[DefaultValue(12)]
		public int SliceCount
		{
			get { return sliceCount; }
			set { sliceCount = value; }
		}
		#endregion
	}

	public enum BlockSize
	{
		XSmall,
		Small,
		Medium,
		Large,
		XLarge,
		XXLarge
	}
}