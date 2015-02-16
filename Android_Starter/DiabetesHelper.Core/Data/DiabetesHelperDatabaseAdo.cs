using System;
using System.Collections.Generic;
using System.IO;
using DiabetesApp.Core.Android.DataAccess;
using DiabetesHelper.Core.DataAccess;
using Exception = Java.Lang.Exception;
using SQLite;
using System.Linq;

namespace DiabetesHelper.Core.Data
{
	public class DiabetesHelperDatabaseAdo : IDiabetesHelperDatabaseAdo
	{
		private readonly string _dbPath;

		public bool SaveMeal (Meal meal)
		{
			if (meal.Name.Length == 0)
				return false;

			return true;
		}

		public void CreateTables ()
		{
			// Drop the tables first.
			DropTables ();
			// Now create the tables.
			using (var connection = new SQLiteConnection (_dbPath)) {
				connection.CreateTable<Meal> ();
				connection.CreateTable<MealEaten> ();
			}
		}

		public MealEaten[] GetAllMealsEaten ()
		{
			using (var connection = new SQLiteConnection (_dbPath)) {
				return connection.Table<MealEaten> ().OrderByDescending (me => me.MealTime).ToArray ();
			}
		}

		private void DropTables ()
		{
			if (File.Exists (_dbPath)) {
				File.Delete (_dbPath);
			}
		}

		public Meal GetMeal (string name)
		{
			return null;
		}

		public DiabetesHelperDatabaseAdo (string dbPath)
		{
			_dbPath = dbPath;
		}

		public void AddMeal (string name, float joules, float carbohydrates)
		{
			using (var connection = new SQLiteConnection (_dbPath)) {
				connection.Insert (new Meal (name, joules, carbohydrates));
			}
		}

		public Meal[] GetAllMeals ()
		{
			using (var connection = new SQLiteConnection (_dbPath)) {
				return connection.Table<Meal>().ToArray();
			}
		}

		public bool DbExists ()
		{
			return File.Exists (_dbPath);
		}

		public void AddMealEaten (int mealId, DateTime time, float bslBefore, float insulinDose, float bslAfter)
		{
			using (var connection = new SQLiteConnection (_dbPath)) {
				connection.Insert (new MealEaten (mealId, time, bslBefore, insulinDose, bslAfter));
			}
		}

		public Meal GetMeal (int mealId)
		{
			using (var connection = new SQLiteConnection (_dbPath)) {
				return connection.Table<Meal> ().FirstOrDefault (m => m.Id == mealId);
			}
		}

		public MealEaten GetMealEaten (int mealEatenId)
		{
			using (var connection = new SQLiteConnection (_dbPath)) {
				return connection.Table<MealEaten> ().FirstOrDefault (m => m.Id == mealEatenId);
			}
		}

		public void UpdateMealEaten (int mealEatenId, int mealId, DateTime time, float bslBefore, float insulinDose, float bslAfter)
		{
			using (var connection = new SQLiteConnection (_dbPath)) {
				connection.Update (new MealEaten (mealId, time, bslBefore, insulinDose, bslAfter) { Id = mealEatenId });
			}
		}

		public void DeleteMealEaten (int mealEatenId)
		{
			using (var connection = new SQLiteConnection (_dbPath)) {
				connection.Delete (new MealEaten { Id = mealEatenId });
			}
		}
	}
}
