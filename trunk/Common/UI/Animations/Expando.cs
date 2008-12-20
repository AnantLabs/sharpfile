using System.Drawing;
using System.Windows.Forms;

namespace Common.UI.Animations {
	public class Expando : AnimationBase {
		public void Start(Control control, Size endSize, AnimationType animationType, int duration) {
			base.Start(control, animationType, duration,
				delegate {
					int width = expando(control.Width, endSize.Width);
					int height = expando(control.Height, endSize.Height);
					Size size = new Size(width, height);

					//control.Width = width;
					//control.Height = height;
					control.Size = size;
				},
				delegate {
					return (control.Size == endSize);
				});
		}

		///<summary>
		///This method returns a value from the expando formula.
		///</summary>
		private int expando(int startPos, int endPos) {
			float t = (float)counter - timeStart;
			float b = (float)startPos;
			float c = (float)endPos - startPos;
			float d = (float)timeDest - timeStart;

			return getFormula(t, b, d, c);
		}
	}
}