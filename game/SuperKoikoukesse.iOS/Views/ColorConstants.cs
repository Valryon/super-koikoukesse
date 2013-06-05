using System;
using MonoTouch.UIKit;

namespace SuperKoikoukesse.iOS
{
	public class ColorConstants
	{
		public static string BRAND_WHITE	= "#D8D8D8";
		public static string BRAND_BLUE 	= "#1F79CA";
		public static string BRAND_GREY 	= "#E6E6E6";
		public static string BRAND_BLACK 	= "#3E3E3E";

		public static string TEXT_WHITE		= "#D8D8D8";
		public static string TEXT_BLACK     = "#4A4A4A";

		public static string COMBO_LEVEL_1  = "#64A4DC";
		public static string COMBO_LEVEL_2  = "#3F99EA";
		public static string COMBO_LEVEL_3  = "#1F79CA";
		public static string COMBO_LEVEL_4  = "#1560A3";

		public static UIColor ToUIColor(string rgbValue) 
		{
			float alpha = 1.0f;
			rgbValue = rgbValue.Replace ("#", "");
			float red   = (float)(Convert.ToInt32(rgbValue.Substring(0, 2), 16))/255f;
			float green = (float)(Convert.ToInt32(rgbValue.Substring(2, 2), 16))/255f;
			float blue  = (float)(Convert.ToInt32(rgbValue.Substring(4, 2), 16))/255f;

			return new UIColor(red, green, blue, alpha);
		}
	}
}

