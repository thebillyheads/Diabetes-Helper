// using Microsoft.Practices.Unity;

using System.Collections.Generic;
using System.IO;
using DiabetesHelper.Core.Data;
using Microsoft.Practices.Unity;
using Environment = System.Environment;
using DiabetesApp.Core.Android.DataAccess;

namespace DiabetesHelper.Core.DataAccess
{
	public class DiabetesHelperRepositoryAdo : IDiabetesHelperRepositoryAdo
    {
		private readonly IDiabetesHelperDatabaseAdo _database;
		private UnityContainer _container;

        public DiabetesHelperRepositoryAdo()
        {
            _container = Global.Container;
			_database = _container.Resolve<IDiabetesHelperDatabaseAdo>(new ParameterOverride("dbPath", DbPath));
        }

        private static string DbPath
        {
            get
            {
                const string sqliteFilename = "DiabetesDatabase.db3";
#if NETFX_CORE
				var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);
#else

#if SILVERLIGHT
    // Windows Phone expects a local path, not absolute
				var path = sqliteFilename;
#else

#if __ANDROID__
                // Just use whatever directory SpecialFolder.Personal returns
                string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
#else
                // we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
                // (they don't want non-user-generated data in Documents)
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
                string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
#endif
                var path = Path.Combine(libraryPath, sqliteFilename);
#endif

#endif
                return path;
            }
        }

        public void CreateTables()
        {
            _database.CreateTables();
        }

        public MealEaten[] GetAllMealsEaten()
        {
            return _database.GetAllMealsEaten();
        }

		public void AddMeal (string name, float joules, float carbohydrates)
		{
			_database.AddMeal(name, joules, carbohydrates);
		}

		public Meal[] GetAllMeals ()
		{
			return _database.GetAllMeals ();
		}

		public bool DbExists ()
		{
			return _database.DbExists ();
		}

		public void AddMealEaten (int mealId, System.DateTime time, float bslBefore, float insulinDose, float bslAfter)
		{
			_database.AddMealEaten (mealId, time, bslBefore, insulinDose, bslAfter);
		}

		public Meal GetMeal (int mealId)
		{
			return _database.GetMeal(mealId);
		}

		public MealEaten GetMealEaten (int mealEatenId)
		{
			return _database.GetMealEaten (mealEatenId);
		}

		public void UpdateMealEaten (int mealEatenId, int mealId, System.DateTime time, float bslBefore, float insulinDose, float bslAfter)
		{
			_database.UpdateMealEaten (mealEatenId, mealId, time, bslBefore, insulinDose, bslAfter);
		}

		public void DeleteMealEaten (int mealEatenId)
		{
			_database.DeleteMealEaten (mealEatenId);
		}
    }
}