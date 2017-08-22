﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;

namespace AutofillFramework.app
{
	[Activity(Label = "CreditCardActivity")]
	public class CreditCardActivity : AppCompatActivity
	{
		static int CC_EXP_YEARS_COUNT = 5;

		Java.Lang.String[] Years = new Java.Lang.String[CC_EXP_YEARS_COUNT];

    	Spinner mCcExpirationDaySpinner;
		Spinner mCcExpirationMonthSpinner;
		Spinner mCcExpirationYearSpinner;
		EditText mCcCardNumber;

		public static Intent GetStartActivityIntent(Context context)
		{
			var intent = new Intent(context, typeof(CreditCardActivity));
        	return intent;
    	}

		class YearAdapter : ArrayAdapter
		{
			Java.Lang.String[] Years { get; set; }
			public YearAdapter(Context context, Java.Lang.String[] years) : base(context, Android.Resource.Layout.SimpleSpinnerItem, years)
			{
				Years = years;
			}

			public override ICharSequence[] GetAutofillOptionsFormatted()
			{
				return Years;
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.credit_card_activity);
			mCcExpirationDaySpinner = (Android.Widget.Spinner)FindViewById(Resource.Id.expirationDay);
			mCcExpirationMonthSpinner = (Android.Widget.Spinner)FindViewById(Resource.Id.expirationMonth);
			mCcExpirationYearSpinner = (Android.Widget.Spinner)FindViewById(Resource.Id.expirationYear);
			mCcCardNumber = (Android.Widget.EditText)FindViewById(Resource.Id.creditCardNumberField);

			// Create an ArrayAdapter using the string array and a default spinner layout
			ArrayAdapter dayAdapter = ArrayAdapter.CreateFromResource
			           (this, Resource.Array.day_array, Android.Resource.Layout.SimpleSpinnerItem);
			// Specify the layout to use when the list of choices appears
			dayAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
			// Apply the adapter to the spinner
			mCcExpirationDaySpinner.Adapter = dayAdapter;

			ArrayAdapter monthAdapter = ArrayAdapter.CreateFromResource
			           (this, Resource.Array.month_array, Android.Resource.Layout.SimpleSpinnerItem);
			monthAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
			mCcExpirationMonthSpinner.Adapter = monthAdapter;
			int year = Calendar.GetInstance(Locale.Default).Get(CalendarField.Year);
			for (int i = 0; i < Years.Length; i++)
			{
				Years[i] = new Java.Lang.String(Integer.ToString(year + i));
			}

			mCcExpirationYearSpinner.Adapter = new YearAdapter(this, Years);
			FindViewById(Resource.Id.submit).Click += (sender, args) => {
				Submit();
			};
			FindViewById(Resource.Id.clear).Click += (sender, args) => {
				ResetFields();
			};
		}

		void ResetFields()
		{
			mCcExpirationDaySpinner.SetSelection(0);
			mCcExpirationMonthSpinner.SetSelection(0);
			mCcExpirationYearSpinner.SetSelection(0);
			mCcCardNumber.Text = "";
		}

		/**
     	 * Launches new Activity and finishes, triggering an autofill save request if the user entered
     	 * any new data.
     	 */
		void Submit()
		{
			Intent intent = WelcomeActivity.GetStartActivityIntent(this);
			StartActivity(intent);
			Finish();
		}
	}
}