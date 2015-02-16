using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DiabetesHelper.Core.DataAccess;
using SQLiteNetExtensions.Attributes;

namespace DiabetesApp.Core.Android.DataAccess
{
	[Table ("Meal")] 
	public class Meal
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Name { get; set; }

		public float Joules { get; set; }

		public float Carbohydrates { get; set; }

		public Meal ()
		{
		}

		public Meal (string name, float joules, float carbohydrates)
		{
			Name = name;
			Joules = joules;
			Carbohydrates = carbohydrates;
		}
	}
}