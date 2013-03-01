using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Superkoikoukesse.Common;
using Superkoikoukesse.Common.Utils;
using System.IO;

namespace SuperKoikoukesse.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{

		// class-level declarations
		UIWindow window;
		GameViewController viewController;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Logger.Log (LogLevel.Info, "Launching app...");

			// Global parameters
			EncryptionHelper.SetKey (Constants.EncryptionKey);
			ImageService.Instance.Initialize (Constants.ImagesRootLocation);

			// Try to open the database
			DatabaseService.Instance.Load(Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), Constants.DatabaseLocation));

			// Create first view
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new GameViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			return true;
		}
	}
}
