
using System;

using Android.App;
using Android.OS;
using Android.Widget;

using DiabetesHelper.Core;
using DiabetesHelper.Core.Business;

using Microsoft.Practices.Unity;
using Android.Content;

namespace DiabetesHelperAndroid
{
	[Activity (Label = "Type of Meal")]			
	public class MealActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.Meal);

			var button = FindViewById(Resource.Id.buttonDone) as Button;
			if (button != null) button.Click += OnDoneButtonClick;
		}

		void OnDoneButtonClick (object sender, EventArgs e)
		{
			EditText text = (EditText) FindViewById(Resource.Id.editTextName);
			string name = text.Text;

			text = (EditText) FindViewById(Resource.Id.editTextCarbohydrates);
			float carbohydrates;
			if (!float.TryParse (text.Text, out carbohydrates))
				carbohydrates = default (float);

			// Save the meal to the database.
			var manager = Global.Container.Resolve<DiabetesHelperManager> ();
			manager.AddMeal(name, default (float), carbohydrates);

			// Close the layout
			Intent intent = new Intent (this, typeof(MealEatenActivity));
			SetResult (Result.Ok, intent);
			base.Finish ();
		}
	}
}

