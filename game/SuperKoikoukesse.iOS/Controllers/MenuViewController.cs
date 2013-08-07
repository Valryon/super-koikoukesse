using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Superkoikoukesse.Common;
using System.IO;
using MonoTouch.GameKit;

namespace SuperKoikoukesse.iOS
{
	public partial class MenuViewController : UIViewController
	{
		#region Fields

		private List<UIViewController> cards;
		private MenuDifficultyViewController difficultyViewController;
		private CardScoreViewController highScorePanel;

		#endregion

		#region Constructor & Initialization

		public MenuViewController ()
			: base ("MenuView"+ (AppDelegate.UserInterfaceIdiomIsPhone ? "_iPhone" : "_iPad"), null)
		{
			cards = new List<UIViewController> ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Hide credits and coins until player profile is loaded
			coinsLabel.Hidden = true;
			creditsLabel.Hidden = true;

			// Localized elements
			if (authorsLabel != null) {
				authorsLabel.Text = Localization.Get ("credits.small");
			}

			var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate; 
			appDelegate.LoadPlayerProfile ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (cards.Count == 0) {
				// We need auto layout to be set up, so we can create panels only here
				CreateCards ();
			}

			debugButton.SetTitle (Constants.DEBUG_MODE + "", UIControlState.Normal);

			UpdateViewWithPlayerInfos ();
		}

		#endregion

		#region Methods

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return AppDelegate.HasSupportedInterfaceOrientations ();
		}

		/// <summary>
		/// Update tickets and coins counters
		/// </summary>
		public void UpdateViewWithPlayerInfos ()
		{
			// Show infos is they were hidden
			if (coinsLabel.Hidden) {
				coinsLabel.Hidden = false;
				creditsLabel.Hidden = false;

				// Fade in
				coinsLabel.Alpha = 0f;
				creditsLabel.Alpha = 0f;

				UIView.Animate (2f, () => {
					coinsLabel.Alpha = 1f;
					creditsLabel.Alpha = 1f;
				});
			}

			// Load the player from db
			Player profile = PlayerCache.Instance.CachedPlayer;

			// Display credits and coins
			if (profile != null) {
				creditsLabel.Text = profile.Credits.ToString ();
				coinsLabel.Text = profile.Coins.ToString ();
			}

			// Update leaderboards ?
			if (highScorePanel != null) {
				highScorePanel.ForceUpdate ();
			}

		}

		#region Scroll view and pagination

		/// <summary>
		/// Create the cards for the menu
		/// </summary>
		private void CreateCards ()
		{
			pageControl.ValueChanged += (object sender, EventArgs e) => {
				SetScrollViewToPage (pageControl.CurrentPage);
			};
			scrollView.DecelerationEnded += (object sender, EventArgs e) => {
				double page = Math.Floor ((scrollView.ContentOffset.X - scrollView.Frame.Width / 2) / scrollView.Frame.Width) + 1;
				
				pageControl.CurrentPage = (int)page;
			};

			highScorePanel = null;
			cards.Clear ();

			// Credits
			CardInfoViewController infos = new CardInfoViewController ();
			cards.Add (infos);

			// Highscores
			highScorePanel = new CardScoreViewController ();
			cards.Add (highScorePanel);

			//
			// Build modes
			//

			// -- Score attack
			CardModeViewController scoreAttackMode = new CardModeViewController (GameMode.SCORE);
			scoreAttackMode.GameModeSelected += HandleGameModeSelected;
			cards.Add (scoreAttackMode);

			// -- Time attack
			CardModeViewController timeAttackMode = new CardModeViewController (GameMode.TIME);
			timeAttackMode.GameModeSelected += HandleGameModeSelected;
			cards.Add (timeAttackMode);

			// -- Survival
			CardModeViewController survivalMode = new CardModeViewController (GameMode.SURVIVAL);
			survivalMode.GameModeSelected += HandleGameModeSelected;
			cards.Add (survivalMode);

			// -- Versus
			CardModeViewController versusMode = new CardModeViewController (GameMode.VERSUS);
			versusMode.GameModeSelected += HandleGameModeSelected;
			cards.Add (versusMode);

			int count = cards.Count;
			RectangleF scrollFrame = scrollView.Frame;

			scrollFrame.Width = scrollFrame.Width * count;
			scrollView.ContentSize = scrollFrame.Size;

			for (int i = 0; i < count; i++) {

				// Compute location and size
				RectangleF frame = scrollView.Frame;

				PointF location = new PointF ();
				location.X = frame.Width * i;
				frame.Location = location;

				cards [i].View.Frame = frame;

				// Add to scroll and paging
				scrollView.AddSubview (cards [i].View);
			}

			pageControl.Pages = count;

			// Set 3rd page as the first displayed
			int firstDisplayedPageNumber = 2;
			pageControl.CurrentPage = firstDisplayedPageNumber;

			SetScrollViewToPage (firstDisplayedPageNumber);
		}

		private void SetScrollViewToPage (int page)
		{
			scrollView.SetContentOffset (new PointF (page * scrollView.Frame.Width, 0), true);
		}

		#endregion

		/// <summary>
		/// Pop-up the Game Center matchmaker
		/// </summary>
		/// <param name="mode">Mode.</param>
		private void DisplayMatchMaker (GameMode mode)
		{
			PlayerCache.Instance.AuthenticatedPlayer.NewMatch (
			// Match found
				(match) => {

				// First turn: choose game parameters
				if (match.IsFirstTurn) {
					DisplayDifficultyChooser (mode);
				} else {
					var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate; 

					if (match.IsEnded) {
						// See the final score
						Dialogs.ShowMatchEnded ();
					} else {
						if (match.IsPlayerTurn (PlayerCache.Instance.AuthenticatedPlayer.PlayerId)) {
							// Player turn
							appDelegate.SwitchToGameView (mode, match.Difficulty, match.Filter);
						} else {
							// TODO Other player turn: display last score?
							Dialogs.ShowNotYourTurn ();
						}
					}
				}
			},
			// Cancel
				() => {
				// Nothing, controller is already dismissed
			},
			// Error
				() => {
				// Display an error dialog?
				UIAlertView alert = new UIAlertView (
					"Une erreur est survenue",
					"Nous n'avons pas pu démarrer une nouvelle partie car une erreur est survenue..",
					null,
					"Ok");
				
				alert.Show ();
			},
			// Player quit
				() => {
				// Kill the game? Inform the player?
			}
			);
		}

		/// <summary>
		/// Pop-up the difficulty chooser
		/// </summary>
		/// <param name="selectedMode">Selected mode.</param>
		private void DisplayDifficultyChooser (GameMode selectedMode)
		{
			// Display difficulty view
			if (difficultyViewController == null) {
				difficultyViewController = new MenuDifficultyViewController ();
				difficultyViewController.DifficultySelected += (GameMode mode, GameDifficulties difficulty) => {
					var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate; 
					
					Filter filter = null;

					// Remember to select Versus match parameters too
					if (mode == GameMode.VERSUS) {
						VersusMatch currentMatch = PlayerCache.Instance.AuthenticatedPlayer.CurrentMatch;

						currentMatch.Difficulty = difficulty;
						filter = currentMatch.Filter; // This is weird
					}
					
					appDelegate.SwitchToGameView (mode, difficulty, filter);
				}; 	
			}
			
			difficultyViewController.SetMode (selectedMode);
			difficultyViewController.View.Frame = View.Frame;
			
			View.AddSubview (difficultyViewController.View);
		}

		void HandleGameModeSelected (GameMode m)
		{
			// Enough credits?
			if (PlayerCache.Instance.CachedPlayer.Credits > 0) {

				if (m == GameMode.VERSUS) {

					if (PlayerCache.Instance.AuthenticatedPlayer.IsAuthenticated == false) {
						PlayerCache.Instance.AuthenticatedPlayer.Authenticate (() => {

							if (PlayerCache.Instance.AuthenticatedPlayer.IsAuthenticated) {
								DisplayMatchMaker (m);
							} else {
								// Dialog
								Dialogs.ShowAuthenticationRequired ();
							}
						});
					} else {
						DisplayMatchMaker (m);
					}
				} else {
					DisplayDifficultyChooser (m);
				}
			} else {
				Dialogs.ShowNoMoreCreditsDialogs ();
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Force config reload (DEBUG)
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void configButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate; 
			appDelegate.UpdateConfiguration (null);
		}

		partial void debugButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			Constants.DEBUG_MODE = !Constants.DEBUG_MODE;
			debugButton.SetTitle (Constants.DEBUG_MODE + "", UIControlState.Normal);
			Logger.I ("Debug mode? " + Constants.DEBUG_MODE);
		}

		partial void paramsButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate; 
			appDelegate.ShowOptions ();
		}

		partial void shopButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate; 
			appDelegate.SwitchToShopView ();
		}

		partial void creditsButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			PlayerCache.Instance.AddCreditsDebug (Constants.BASE_CREDITS);
		}

		partial void coinsButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			PlayerCache.Instance.AddCoins (Constants.BASE_COINS);
		}

		#endregion
	}
}

