using System.Windows.Forms;

namespace Common.UI.Animations {
	public interface IAnimationBase {
		void Start(Control control, AnimationType animationType, int duration,
			AnimationBase.StartActionDelegate startAction, 
			AnimationBase.CompleteEvaluatorDelegate completeEvaluator);

		bool Complete { get; }
	}
}