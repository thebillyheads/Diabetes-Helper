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

namespace DiabetesHelper.Core.DataAccess
{
    class InsulinDose
    {
        public const string TableSqlCreateCommand = "CREATE TABLE [InsulinDoses] (Id INTEGER PRIMARY KEY ASC,MealEatenId INTEGER NOT NULL,Dose FLOAT NOT NULL,FOREIGN KEY(MealEatenId) REFERENCES MealsEaten(Id))";
    }
}