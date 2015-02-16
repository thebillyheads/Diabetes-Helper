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

namespace DiabetesHelper.Core.DataAccess
{
    interface IDiabetesHelperRepositoryAdo
    {
        void CreateTables();

        MealEaten[] GetAllMealsEaten();

		void AddMeal (string name, float joules, float carbohydrates);

		Meal[] GetAllMeals ();

		bool DbExists ();

		void AddMealEaten (int mealId, DateTime time, float bslBefore, float insulinDose, float bslAfter);

		Meal GetMeal (int mealId);

		MealEaten GetMealEaten (int mealEatenId);

		void UpdateMealEaten (int mealEatenId, int mealId, DateTime time, float bslBefore, float insulinDose, float bslAfter);

		void DeleteMealEaten (int mealEatenId);
    }
}