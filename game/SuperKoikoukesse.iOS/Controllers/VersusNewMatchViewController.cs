using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Superkoikoukesse.Common;

namespace SuperKoikoukesse.iOS
{
  public partial class VersusNewMatchViewController : UIViewController
  {
    private static string VersusToGameSegueId = "VersusNewToGame";
    private GameLauncher mGameLauncher;

    public VersusNewMatchViewController(IntPtr handle) : base (handle)
    {
    }

    partial void OnGoTouched(MonoTouch.Foundation.NSObject sender)
    {
      PlayerCache.Instance.AuthenticatedPlayer.NewMatch(
				// Match found
        (match) => {

        // Ensure it's a new match
        if (match.IsFirstTurn)
        {
          mGameLauncher = new GameLauncher(this);
          mGameLauncher.Launch(VersusToGameSegueId, GameMode.SCORE, GameDifficulties.NORMAL, new Filter());
        } 
//					else {
//						var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate; 
//
//						if (match.IsEnded) {
//							// See the final score
//							Dialogs.ShowMatchEnded ();
//						} else {
//							if (match.IsPlayerTurn (PlayerCache.Instance.AuthenticatedPlayer.PlayerId)) {
//								// Player turn
//								LaunchGame (mode, match.Difficulty, match.Filter);
//							} else {
//								// TODO Other player turn: display last score?
//								Dialogs.ShowNotYourTurn ();
//							}
//						}
//					}
      },
			// Cancel
        () => {
        // Nothing, controller is already dismissed
      },
			// Error
        () => {
        // Display an error dialog?
        UIAlertView alert = new UIAlertView(
          "Une erreur est survenue",
          "Nous n'avons pas pu démarrer une nouvelle partie car une erreur est survenue..",
          null,
          "Ok");

        alert.Show();
      },
			// Player quit
        () => {
        // Kill the game? Inform the player?
      }
      );

    }

    public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
    {
      base.PrepareForSegue(segue, sender);

      if (mGameLauncher != null && segue.Identifier == mGameLauncher.SegueId)
      {
        GameViewController gameVc = (GameViewController) segue.DestinationViewController;

        // Prepare quizz
        mGameLauncher.Prepare(gameVc);
      }
    }
  }
}