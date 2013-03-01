using System;
using SQLite;

namespace Superkoikoukesse.Common
{
	/// <summary>
	/// Game entry in the database
	/// </summary>
	public class GameInfo
	{
		/// <summary>
		/// Id in the server database
		/// </summary>
		/// <value>The game identifier.</value>
		[PrimaryKey]
		public int GameId { get; set; }

		/// <summary>
		/// European title
		/// </summary>
		public string TitlePAL { get; set; }

		/// <summary>
		/// American title
		/// </summary>
		public string TitleUS { get; set; }

		/// <summary>
		/// Path to the image
		/// </summary>
		/// <value>The image path.</value>
		public string ImagePath {
			get;
			set;
		}

		/// <summary>
		/// Game platform
		/// </summary>
		/// <value>The platform.</value>
		[Indexed]
		public string Platform {
			get;
			set;
		}

		/// <summary>
		/// Game genre
		/// </summary>
		/// <value>The genre.</value>
		[Indexed]
		public string Genre {
			get;
			set;
		}

		/// <summary>
		/// Game publisher
		/// </summary>
		/// <value>The publisher.</value>
		public string Publisher {
			get;
			set;
		}

		/// <summary>
		/// Game year of publishing
		/// </summary>
		/// <value>The year.</value>
		public int Year {
			get;
			set;
		}


		public GameInfo ()
		{
		}

		public override string ToString ()
		{
			return GameId + " " + TitlePAL;
		}
	}
}
