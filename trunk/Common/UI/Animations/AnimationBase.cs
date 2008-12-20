using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Common.UI.Animations {
	///<summary>
	///  
	///</summary>
	public class AnimationBase : IAnimationBase {
		public event AnimationCompleteDelegate AnimationComplete;
		public delegate void AnimationCompleteDelegate();

		public delegate void StartActionDelegate();
		public delegate bool CompleteEvaluatorDelegate();

		protected int counter;
		protected int timeStart;
		protected int timeDest;
		protected AnimationType animationType;
		protected int interval = 1;
		private Control control;
		private IContainer components;
		private Timer timer;
		private bool complete = false;
		private StartActionDelegate startAction;
		private CompleteEvaluatorDelegate completeEvaluator;

		public AnimationBase() {
			components = new Container();
			timer = new Timer(this.components);
		}

		public void Start(Control control, int interval, StartActionDelegate startAction, 
			CompleteEvaluatorDelegate completeEvaluator) {
			this.interval = interval;
			Start(control, AnimationType.NotApplicable, 0, startAction, completeEvaluator);
		}

		public void Start(Control control, AnimationType animationType, int duration, 
			StartActionDelegate startAction, CompleteEvaluatorDelegate completeEvaluator) {
			this.control = control;
			this.animationType = animationType;
			this.startAction = startAction;
			this.completeEvaluator = completeEvaluator;

			// Inits the parameters for the animation process.
			counter = 0;
			timeStart = counter;
			timeDest = duration;

			timer.Interval = interval;
			timer.Tick += timer_Tick;

			// Start the timer.
			timer.Stop();
			timer.Enabled = false;
			timer.Enabled = true;
		}

		///<summary>
		///This is the method that gets called every tick interval
		///</summary>
		private void timer_Tick(object sender, EventArgs e) {
			if (completeEvaluator == null || completeEvaluator.Invoke()) {
				timer.Stop();
				timer.Enabled = false;
				complete = true;

				if (AnimationComplete != null) {
					AnimationComplete();
				}
			} else {
				startAction.Invoke();
				counter++;
			}
		}

		protected int getFormula(float t, float b, float d, float c) {
			switch (animationType) {
				case AnimationType.Linear:
					// simple linear tweening - no easing 
					return (int)(c * t / d + b);

				case AnimationType.EaseInQuad:
					// quadratic (t^2) easing in - accelerating from zero velocity
					return (int)(c * (t /= d) * t + b);

				case AnimationType.EaseOutQuad:
					// quadratic (t^2) easing out - decelerating to zero velocity
					return (int)(-c * (t = t / d) * (t - 2) + b);

				case AnimationType.EaseInOutQuad:
					// quadratic easing in/out - acceleration until halfway, then deceleration
					if ((t /= d / 2) < 1) {
						return (int)(c / 2 * t * t + b);
					} else {
						return (int)(-c / 2 * ((--t) * (t - 2) - 1) + b);
					}

				case AnimationType.EaseInCubic:
					// cubic easing in - accelerating from zero velocity
					return (int)(c * (t /= d) * t * t + b);

				case AnimationType.EaseOutCubic:
					// cubic easing in - accelerating from zero velocity
					return (int)(c * ((t = t / d - 1) * t * t + 1) + b);

				case AnimationType.EaseInOutCubic:
					// cubic easing in - accelerating from zero velocity
					if ((t /= d / 2) < 1) {
						return (int)(c / 2 * t * t * t + b);
					} else {
						return (int)(c / 2 * ((t -= 2) * t * t + 2) + b);
					}

				case AnimationType.EaseInQuart:
					// quartic easing in - accelerating from zero velocity
					return (int)(c * (t /= d) * t * t * t + b);

				case AnimationType.EaseInExpo:
					// exponential (2^t) easing in - accelerating from zero velocity
					if (t == 0) {
						return (int)b;
					} else {
						return (int)(c * Math.Pow(2, (10 * (t / d - 1))) + b);
					}

				case AnimationType.EaseOutExpo:
					// exponential (2^t) easing out - decelerating to zero velocity
					if (t == d) {
						return (int)(b + c);
					} else {
						return (int)(c * (-Math.Pow(2, -10 * t / d) + 1) + b);
					}

				default:
					return 0;
			}
		}

		public bool Complete {
			get {
				return complete;
			}
		}
	}
}