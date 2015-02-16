
using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DiabetesHelper.Core;
using DiabetesHelper.Core.Business;
using Microsoft.Practices.Unity;
using DiabetesApp.Core.Android.DataAccess;
using DiabetesHelper.Core.DataAccess;

namespace DiabetesHelperAndroid
{
	[Activity (Label = "Meal, BSLs and Insulin Dosage")]			
	public class MealEatenActivity : Activity
	{
		const int MealActivityResultCode = 0;

		private Meal[] _meals;
		private Spinner _spinner;
		private DatePicker _datePickerTime;
		private TimePicker _timePickerTime;
		private EditText _bslBefore;
		private EditText _insulinDose;
		private EditText _bslAfter;

		private int _mealEatenId;

		DiabetesHelperManager _manager;

		private void LoadMealSpinner ()
		{
			_meals = _manager.GetAllMeals ();
			ArrayAdapter<string> adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleSpinnerItem, _meals.Select (m => m.Name).ToArray ());
			_spinner.Adapter = adapter;
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			_manager = Global.Container.Resolve<DiabetesHelperManager> ();

			SetContentView (Resource.Layout.MealEaten);

			_spinner = FindViewById (Resource.Id.spinnerMeal) as Spinner;

			_datePickerTime = FindViewById (Resource.Id.datePickerTime) as DatePicker;
			_timePickerTime = FindViewById (Resource.Id.timePickerTime) as TimePicker;
			_timePickerTime.SetIs24HourView ((Java.Lang.Boolean)true);

			_bslBefore = (EditText)FindViewById (Resource.Id.editTextBslBefore);
			_insulinDose = (EditText)FindViewById (Resource.Id.editTextInsulinDose);
			_bslAfter = (EditText)FindViewById (Resource.Id.editTextBslAfter);

			var button = FindViewById (Resource.Id.buttonAddMeal) as Button;
			if (button != null)
				button.Click += OnAddMealButtonClick;

			button = FindViewById (Resource.Id.buttonDone) as Button;
			if (button != null)
				button.Click += OnDoneButtonClick;

			// Populate the meal spinner control.
			LoadMealSpinner ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			_mealEatenId = Intent.GetIntExtra ("MealEatenId", 0);
			if (_mealEatenId > 0) {
				// An existing meal eaten is being edited.
				var mealEaten = _manager.GetMealEaten (_mealEatenId);

				// Set the spinner value
				var mealName = _meals.First (m => m.Id == mealEaten.MealId).Name;
				for (int i = 0; i < +_spinner.Adapter.Count; i++) {
					if ((string)_spinner.Adapter.GetItem (i) == mealName) {
						_spinner.SetSelection (i);
						break;
					}
				}
				_datePickerTime.DateTime = mealEaten.MealTime.Date;
				_timePickerTime.CurrentHour = (Java.Lang.Integer)mealEaten.MealTime.TimeOfDay.Hours;
				_timePickerTime.CurrentMinute = (Java.Lang.Integer)mealEaten.MealTime.TimeOfDay.Minutes;

				_bslBefore.Text = mealEaten.BslBefore.ToString ();
				_insulinDose.Text = mealEaten.InsulinDose.ToString ();
				_bslAfter.Text = mealEaten.BslAfter.ToString ();
			} else {
				DateTime localNow = DateTime.Now;
				_datePickerTime.DateTime = localNow;

				_timePickerTime.CurrentHour = (Java.Lang.Integer)localNow.Hour;
				_timePickerTime.CurrentMinute = (Java.Lang.Integer)localNow.Minute;

				_bslBefore.Text = string.Empty;
				_insulinDose.Text = string.Empty;
				_bslAfter.Text = string.Empty;
			}
		}

		void OnAddMealButtonClick (object sender, EventArgs e)
		{
			Intent mealActivity = new Intent (this, typeof(MealActivity));
			StartActivityForResult (mealActivity, MealActivityResultCode);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == MealActivityResultCode && resultCode == Result.Ok)
				LoadMealSpinner ();
		}

		int GetSelectedMealId ()
		{
			if (_meals == null)
				return 0;

			var spinner = FindViewById (Resource.Id.spinnerMeal) as Spinner;
			Meal meal = _meals.First (m => m.Name == (string)spinner.SelectedItem);
			return meal != null ? meal.Id : 0;
		}

		void OnDoneButtonClick (object sender, EventArgs e)
		{
			var spinner = FindViewById (Resource.Id.spinnerMeal) as Spinner;
			int mealId = GetSelectedMealId ();

			DateTime time = new DateTime (_datePickerTime.DateTime.Year, _datePickerTime.DateTime.Month, _datePickerTime.DateTime.Day, (int)_timePickerTime.CurrentHour, (int)_timePickerTime.CurrentMinute, 0);

			float bslBefore;
			if (!float.TryParse (_bslBefore.Text, out bslBefore))
				bslBefore = default (float);

			float insulinDose;
			if (!float.TryParse (_insulinDose.Text, out insulinDose))
				insulinDose = default (float);

			float bslAfter;
			if (!float.TryParse (_bslAfter.Text, out bslAfter))
				bslAfter = default (float);

			// Add/update the meal eaten to/in the database.
			if (_mealEatenId > 0)
				_manager.UpdateMealEaten (_mealEatenId, mealId, time, bslBefore, insulinDose, bslAfter);
			else
				_manager.AddMealEaten (mealId, time, bslBefore, insulinDose, bslAfter);

			// Close the layout
			Intent intent = new Intent (this, typeof(MainActivity));
			SetResult (Result.Ok, intent);
			base.Finish ();
		}
	}
}

