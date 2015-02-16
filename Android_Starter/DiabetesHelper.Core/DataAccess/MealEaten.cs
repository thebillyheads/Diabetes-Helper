using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DiabetesApp.Core.Android.DataAccess;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DiabetesHelper.Core.DataAccess
{
	[Table ("MealEaten")]
	public class MealEaten
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[ForeignKey (typeof(Meal))]
		public int MealId { get; set; }

		public DateTime MealTime {
			get;
			set;
		}

		public float InsulinDose {
			get;
			set;
		}

		public float BslBefore {
			get;
			set;
		}

		public float BslAfter {
			get;
			set;
		}

		public MealEaten ()
		{
		}

		public MealEaten (int mealId, DateTime mealTime, float bslBefore, float insulineDose, float bslAfter)
		{
			MealId = mealId;
			MealTime = mealTime;
			BslBefore = bslBefore;
			InsulinDose = insulineDose;
			BslAfter = bslAfter;
		}
	}
}