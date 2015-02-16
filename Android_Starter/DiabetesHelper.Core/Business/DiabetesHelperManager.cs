using System.Collections.Generic;
using DiabetesHelper.Core.Data;
using DiabetesHelper.Core.DataAccess;
using Microsoft.Practices.Unity;
using DiabetesApp.Core.Android.DataAccess;
using System;

namespace DiabetesHelper.Core.Business
{
    public class DiabetesHelperManager
    {
		private readonly IDiabetesHelperRepositoryAdo _repository;
        private UnityContainer _container;

        public DiabetesHelperManager()
        {
            // Configure the DI.
            _container = Global.Container;
            _container.RegisterType<IDiabetesHelperRepositoryAdo, DiabetesHelperRepositoryAdo>();
            _container.RegisterType<IDiabetesHelperDatabaseAdo, DiabetesHelperDatabaseAdo>();

            _repository = _container.Resolve<IDiabetesHelperRepositoryAdo>();
        }

        public void CreateTables()
        {
            _repository.CreateTables();
        }

        public MealEaten[] GetAllMealsEaten()
        {
            return _repository.GetAllMealsEaten();
        }

		public void AddMeal (string name, float joules, float carbohydrates)
		{
			_repository.AddMeal(name, joules, carbohydrates);
		}

		public Meal[] GetAllMeals ()
		{
			return _repository.GetAllMeals ();
		}

		public bool DbExists ()
		{
			return _repository.DbExists ();
		}

		public void AddMealEaten (int mealId, DateTime time, float bslBefore, float insulinDose, float bslAfter)
		{
			_repository.AddMealEaten(mealId, time, bslBefore, insulinDose, bslAfter);
		}

		public Meal GetMeal (int mealId)
		{
			return _repository.GetMeal (mealId);
		}

		public MealEaten GetMealEaten (int mealEatenId)
		{
			return _repository.GetMealEaten (mealEatenId);
		}

		public void UpdateMealEaten (int mealEatenId, int mealId, DateTime time, float bslBefore, float insulinDose, float bslAfter)
		{
			_repository.UpdateMealEaten(mealEatenId, mealId, time, bslBefore, insulinDose, bslAfter);
		}

		public void DeleteMealEaten (int mealEatenId)
		{
			_repository.DeleteMealEaten (mealEatenId);
		}
    }
}