
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SuperKoikoukesse.iOS
{
	public partial class HelpGeneralViewController : UIViewController
	{
		static bool UserInterfaceIdiomIsPhone {
//			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
			get { return true; }
		}

		public HelpGeneralViewController ()
			: base (UserInterfaceIdiomIsPhone ? "HelpGeneralView_iPhone" : "HelpGeneralView_iPad", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		partial void backButtonPressed (MonoTouch.Foundation.NSObject sender) {
			this.View.RemoveFromSuperview();
		}
	}
}

