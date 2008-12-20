using System.Drawing;
using System.Windows.Forms;

namespace Common.UI.Animations {
	public class Tween : AnimationBase {
		public void Start(Control control, Point endPoint, AnimationType animationType, int duration) {
			base.Start(control, animationType, duration, 
				delegate {
					int x = tween(control.Location.X, endPoint.X);
					int y = tween(control.Location.Y, endPoint.Y);
					Point point = new Point(x, y);

					control.Location = point;
				}, 
				delegate {
					return (control.Location == endPoint);
				});
		}

		///<summary>
		///This method returns a value from the tween formula.
		///</summary>
		private int tween(int startPos, int endPos) {
			float t = (float)counter - timeStart;
			float b = (float)startPos;
			float c = (float)endPos - startPos;
			float d = (float)timeDest - timeStart;

			return getFormula(t, b, d, c);
		}
	}
}