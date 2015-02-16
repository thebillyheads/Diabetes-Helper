using System.Collections.Generic;
using DiabetesApp.Core.Android.DataAccess;
using DiabetesHelper.Core.DataAccess;

namespace DiabetesHelper.Core.Data
{
    public interface IDiabetesHelperDatabaseAdo
    {
        bool SaveMeal(Meal testMeal);

        void CreateTables();

        MealEaten[] GetAllMealsEaten();

		void AddMeal (string name, float joules, float carbohydrates);

		Meal[] GetAllMeals ();

		bool DbExists ();

		void AddMealEaten (int mealId, System.DateTime time, float bslBefore, float insulinDose, float bslAfter);

		Meal GetMeal (int mealId);

		MealEaten GetMealEaten (int mealEatenId);

		void UpdateMealEaten (int mealEatenId, int mealId, System.DateTime time, float bslBefore, float insulinDose, float bslAfter);

		void DeleteMealEaten (int mealEatenId);
    }
}