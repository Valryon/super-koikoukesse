using System;
using System.Json;
using System.Web;

namespace Superkoikoukesse.Common
{
	/// <summary>
	/// Player information (id, credits, coins)
	/// </summary>
	public class Player : IServiceOutput
	{
		/// <summary>
		/// Unique identifier for the player (game center id)
		/// </summary>
		/// <value>The identifier.</value>
		public string Id { get; set; }

		/// <summary>
		/// Player name to use
		/// </summary>
		/// <value>The display name.</value>
		public string DisplayName { get; set; }

		/// <summary>
		/// Credits left
		/// </summary>
		/// <value>The credits.</value>
		public int Credits { get; set; }

		/// <summary>
		/// Subscription type (TODO)
		/// </summary>
		/// <value>The type of the subscription.</value>
		public int SubscriptionType { get; set; }

		/// <summary>
		/// Earned coins
		/// </summary>
		/// <value>The coins.</value>
		public long Coins { get; set; }

		/// <summary>
		/// Last time player credits has been upgraded
		/// </summary>
		/// <value>The last credits update.</value>
		public DateTime LastCreditsUpdate { get; set; }

		/// <summary>
		/// Credits used while disconnected
		/// </summary>
		/// <value>The disconnected credits used.</value>
		public int DisconnectedCreditsUsed  { get; set; }

		/// <summary>
		/// Coins earned while disconnected
		/// </summary>
		/// <value>The disconnected coins earned.</value>
		public int DisconnectedCoinsEarned  { get; set; }

		public Player ()
		{
			Credits = Constants.BaseCredits;
			Coins = Constants.BaseCoins;
		}

		public Player (AuthenticatedPlayer aplayer)
			: this()
		{
			DisplayName = aplayer.DisplayName;

			// Clean ID from URL reserved chars
			Id = CleanId(aplayer.PlayerId);
		}

		public void BuildFromJsonObject (JsonValue json)
		{
			//json	{"id": "G1728633519", "date": "2013-03-27T11:22:51.96Z", "credits": 2500, "coins": 3}
			string playerId = json ["id"].ToString ();
			int credits = Convert.ToInt32 (json ["credits"].ToString ());
			int coins = Convert.ToInt32 (json ["coins"].ToString ());
//			int subscriptionType = Convert.ToInt32 (json ["SubscriptionType"].ToString ());

			this.Id = playerId;
			this.Credits = credits;
			this.Coins = coins;
			this.SubscriptionType = 0; // TODO
		}

		/// <summary>
		/// Cleans the identifier.
		/// </summary>
		/// <returns>The identifier.</returns>
		/// <param name="playerId">Player identifier.</param>
		public static string CleanId(string playerId) {
			return playerId.Replace (":", "").Replace ("&", "").Replace ("/", "").Replace (" ", "");
		}
	}
}

