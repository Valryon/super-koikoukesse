// Copyright © 2013 Pixelnest Studio
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package.
using System;
using System.Collections.Generic;
using System.Json;

namespace Superkoikoukesse.Common
{
	/// <summary>
	/// Played turn for the a match
	/// </summary>
	public class VersusMatchTurn
	{
		public int Score { get; set; }

		public string PlayerId { get; set; }
	}

	/// <summary>
	/// Match data for multiplayer mode
	/// </summary>
	public class VersusMatch
	{
		public VersusMatch ()
		{
			Turns = new List<VersusMatchTurn> ();
			IsEnded = false;
		}

		/// <summary>
		/// Serialize the match data in json
		/// </summary>
		/// <returns>The json.</returns>
		public string ToJson ()
		{
			JsonObject json = new JsonObject ();

			json.Add (new KeyValuePair<string, JsonValue> ("MatchId", new JsonPrimitive (MatchId)));
			json.Add (new KeyValuePair<string, JsonValue> ("IsEnded", new JsonPrimitive (IsEnded)));
			json.Add (new KeyValuePair<string, JsonValue> ("Player1Id", new JsonPrimitive (Player1Id)));
			json.Add (new KeyValuePair<string, JsonValue> ("Player2Id", new JsonPrimitive (Player2Id)));
			json.Add (new KeyValuePair<string, JsonValue> ("Difficulty", Difficulty.ToString()));
			json.Add (new KeyValuePair<string, JsonValue> ("Filter", Filter.ToJson ()));

			JsonArray turnsJson = new JsonArray ();
			foreach (VersusMatchTurn turn in Turns) {
				JsonObject turnJson = new JsonObject ();

				turnJson.Add (new KeyValuePair<string, JsonValue> ("PlayerId", new JsonPrimitive (turn.PlayerId)));
				turnJson.Add (new KeyValuePair<string, JsonValue> ("Score", new JsonPrimitive (turn.Score)));

				turnsJson.Add (turnJson);
			}

			json.Add (new KeyValuePair<string, JsonValue> ("Turns", turnsJson));

			return json.ToString ();

		}

		/// <summary>
		/// Read match json and fill the current object
		/// </summary>
		public void FromJson (string jsonRaw)
		{
			JsonValue json = JsonObject.Parse(jsonRaw);

			MatchId = json ["MatchId"];
			Player1Id = json ["Player1Id"];
			Player2Id = json ["Player2Id"];
			IsEnded = Convert.ToBoolean(json ["IsEnded"].ToString());
			Difficulty =  (GameDifficulty)Enum.Parse (typeof(GameDifficulty), json["Difficulty"]);

			Filter = new Filter (json ["Filter"]);

			Turns.Clear ();
			if (json ["Turns"] != null && json ["Turns"] is JsonArray) {
				foreach (JsonObject turnJson in ((JsonArray)json["Turns"])) {

					int score = Convert.ToInt32 (turnJson ["Score"].ToString());
					string playerId = turnJson ["PlayerId"];

					Turns.Add (new VersusMatchTurn () {
					PlayerId = playerId,
					Score = score
				});
				}
			}
		}

		/// <summary>
		/// Check if the given player should play or wait
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		public bool IsPlayerTurn(string playerId) {

			// First turn: player who create the match starts
			if(Turns.Count == 0) return Player1Id == playerId;

			// Get last turn
			VersusMatchTurn lastTurn = Turns[Turns.Count - 1];

			// Check if this is the other player
			return lastTurn.PlayerId != playerId;
		}
		
		/// <summary>
		/// First turn of the match?
		/// </summary>
		public bool IsFirstTurn {
			get {
				return (Turns.Count == 0) ;
			}
		}

		public bool IsEnded { get; set; }

		public string MatchId { get; set; }

		public Filter Filter { get; set; }

		public string Player1Id { get; set; }

		public string Player2Id { get; set; }

		public List<VersusMatchTurn>  Turns  { get; set; }

		public GameDifficulty Difficulty { get; set; }

#if IOS
    public MonoTouch.GameKit.GKTurnBasedMatch GKMatch { get; set; }
#endif
	}
}

