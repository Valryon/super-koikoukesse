// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SuperKoikoukesse.iOS
{
	[Register ("MenuViewController")]
	partial class MenuViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton creditsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView bgImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton scoreAttackButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton timeAttackButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton survivalButon { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton debugButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton helpButton { get; set; }

		[Action ("creditsButtonPressed:")]
		partial void creditsButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("scoreAttackButtonPressed:")]
		partial void scoreAttackButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("timeAttackButtonPressed:")]
		partial void timeAttackButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("survivalButtonPressed:")]
		partial void survivalButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("configButtonPressed:")]
		partial void configButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("debugButtonPressed:")]
		partial void debugButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("helpButtonPressed:")]
		partial void helpButtonPressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (creditsButton != null) {
				creditsButton.Dispose ();
				creditsButton = null;
			}

			if (bgImage != null) {
				bgImage.Dispose ();
				bgImage = null;
			}

			if (scoreAttackButton != null) {
				scoreAttackButton.Dispose ();
				scoreAttackButton = null;
			}

			if (timeAttackButton != null) {
				timeAttackButton.Dispose ();
				timeAttackButton = null;
			}

			if (survivalButon != null) {
				survivalButon.Dispose ();
				survivalButon = null;
			}

			if (debugButton != null) {
				debugButton.Dispose ();
				debugButton = null;
			}

			if (helpButton != null) {
				helpButton.Dispose ();
				helpButton = null;
			}
		}
	}
}
