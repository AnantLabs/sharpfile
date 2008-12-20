using System.Windows.Forms;

namespace Common.UI.Animations {
	public class Fader : AnimationBase {
		public void Start(Form form, double endOpacity, int interval) {
			base.Start(form, interval,
				delegate {
					form.Opacity += 0.1;
					form.Invalidate();
				},
				delegate {
					return (form.Opacity >= endOpacity);
				});
		}
	}
}