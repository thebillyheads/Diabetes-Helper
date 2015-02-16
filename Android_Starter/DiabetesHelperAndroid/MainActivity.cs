using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using DiabetesHelper.Core;
using DiabetesHelper.Core.Business;
using DiabetesHelper.Core.DataAccess;
using Microsoft.Practices.Unity;
using Android.Content;
using DiabetesApp.Core.Android.DataAccess;

namespace DiabetesHelperAndroid
{
	[Activity (Label = "Diabetes Helper", MainLauncher = true, Icon = "@drawable/diabetes")]
	public class MainActivity : Activity
	{
		const int MealsEatenActivityResultCode = 0;

		private UnityContainer _container;
		private DiabetesHelperManager _manager;

		const int NoFilterPosition = 0;
		int _mealSpinnerPosition = NoFilterPosition;

		private void RefreshControls (bool mealSpinner = true, bool mealsEatenTable = true)
		{
			MealEaten[] mealsEaten = _manager.GetAllMealsEaten ();
			if (mealSpinner)
				LoadMealSpinner (mealsEaten);

			if (mealsEatenTable)
				LoadMealsEatenTable (ApplyFilter (mealsEaten));
		}

		MealEaten[] ApplyFilter (MealEaten[] mealsEaten)
		{
			if (_mealSpinnerPosition != NoFilterPosition) {
				var spinner = FindViewById (Resource.Id.spinnerMeal) as Spinner;
				return mealsEaten.Where (me => {
					Meal meal = _manager.GetMeal (me.MealId);
					string filteredMealName = (string)spinner.Adapter.GetItem (_mealSpinnerPosition);
					return meal.Name == filteredMealName;
				}).ToArray ();
			}

			// No filter applied.
			return mealsEaten;
		}

		protected override void OnCreate (Bundle bundle)
		{
			_container = Global.Container;
			_container.RegisterType<DiabetesHelperManager> ();
			_manager = _container.Resolve<DiabetesHelperManager> ();

			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			if (_manager.DbExists ())
				RefreshControls ();
			else
				_manager.CreateTables ();

			var spinner = FindViewById (Resource.Id.spinnerMeal) as Spinner;
			spinner.ItemSelected += (sender, args) => {
				if (args.Position != _mealSpinnerPosition) {
					_mealSpinnerPosition = args.Position;
					RefreshControls (false);
				}
			};
		}

		private void LoadMealsEatenTable (MealEaten[] mealsEaten)
		{
			Action<string, int, TableRow> addColumn = (text, fontSize, to) => {
				var textView = new TextView (this);
				textView.SetText (text, TextView.BufferType.Normal);
				textView.SetTextSize (ComplexUnitType.Pt, fontSize);
				to.AddView (textView);
			};

			TableLayout tableLayout = FindViewById (Resource.Id.tableLayoutMealsEaten) as TableLayout;

			Action<int> addSpacer = (height) => {
				var view = new View (this);
				view.SetMinimumHeight (height);
				view.SetBackgroundColor (Android.Graphics.Color.White);
				tableLayout.AddView (view);
			};

			// Clear the table layout
			tableLayout.RemoveAllViews ();

			// Add the header row
			const int headerFontSize = 6;
			var row = new TableRow (this);
			addColumn ("Time", headerFontSize, row);
			addColumn ("Meal", headerFontSize, row);
			addColumn ("BSL Before", headerFontSize, row);
			addColumn ("Insulin Dose", headerFontSize, row);
			addColumn ("BSL After", headerFontSize, row);
			tableLayout.AddView (row);
			addSpacer (2);

			foreach (var mealEaten in mealsEaten) {
				Meal meal = _manager.GetMeal (mealEaten.MealId);
				if (meal == null)
					continue;

				// Add the row with its columns
				const int normalFontSize = 6;
				row = new TableRow (this);
				addColumn (mealEaten.MealTime.ToString ("g"), normalFontSize, row);
				addColumn (meal.Name, normalFontSize, row);
				addColumn (mealEaten.BslBefore.ToString ("N1"), normalFontSize, row);
				addColumn (mealEaten.InsulinDose.ToString ("N1"), normalFontSize, row);
				addColumn (mealEaten.BslAfter.ToString ("N1"), normalFontSize, row);

				// Add a long click handler for the row
				row.Click += (sender, args) => LaunchMealEatenActivity (mealEaten.Id);
				row.LongClick += (sender, e) => DeleteMealEaten (mealEaten.Id);

				tableLayout.AddView (row);
				addSpacer (1);
			}
		}

		private void LoadMealSpinner (MealEaten[] mealsEaten)
		{
			var distinctMealsEaten = mealsEaten.GroupBy (me => me.MealId).Select (me => me.First ());
			var spinner = FindViewById (Resource.Id.spinnerMeal) as Spinner;
			ArrayAdapter<string> adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleSpinnerItem, distinctMealsEaten.Select 
					(me => _manager.GetMeal (me.MealId).Name).ToList ());
			// Clears the filter
			adapter.Insert (string.Empty, NoFilterPosition);
			spinner.Adapter = adapter;
		}

		private void LaunchMealEatenActivity (int mealEatenId = 0)
		{
			Intent mealEatenIntent = new Intent (this, typeof(MealEatenActivity));
			mealEatenIntent.PutExtra ("MealEatenId", mealEatenId);
			StartActivityForResult (mealEatenIntent, MealsEatenActivityResultCode);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == MealsEatenActivityResultCode && resultCode == Result.Ok)
				RefreshControls ();
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			menu.Add (0, 0, 0, "Add a Meal");
			menu.Add (1, 0, 0, "Create Tables");
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.GroupId) {
			case 0:
				switch (item.ItemId) {
				case 0:
					LaunchMealEatenActivity ();
					return true;
				default:
					break;
				}
				break;
			case 1:
				switch (item.ItemId) {
				case 0:
					AlertDialog alertDialog = new AlertDialog.Builder (this).Create ();
					alertDialog.SetTitle ("Create Tables");
					alertDialog.SetMessage ("Are you sure you want to create a new database? This will wipe all existing data.");
					alertDialog.SetButton ("Yes", (sender, e) => {
						_manager.CreateTables ();
						RefreshControls ();
					});
					alertDialog.Show ();
					return true;
				default:
					break;
				}
				break;
			default:
				break;
			}
			return base.OnOptionsItemSelected (item);
		}

		private void DeleteMealEaten (int mealEatenId)
		{
			AlertDialog alertDialog = new AlertDialog.Builder (this).Create ();
			alertDialog.SetTitle ("Detele Meal");
			alertDialog.SetMessage ("Delete?");
			alertDialog.SetButton ("Yes", (sender, e) => {
				_manager.DeleteMealEaten(mealEatenId);
				RefreshControls ();
			});
			alertDialog.Show ();
		}
	}
}

